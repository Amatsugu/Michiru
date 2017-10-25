using Michiru.Calculation;
using Michiru.Neural;
using Utils;
using Michiru.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SkiaSharp;
using System;

namespace ChiruApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ChiruMatrix _trainX, _testX;
		private ChiruMatrix _trainY, _testY, _predictY;
		private Parameters _parameters = null;
		private bool _willStop = false;
		private bool _isRunning = false;
		private bool _hasData = false;
		private const string StandbyMessage = "Ready!";
		private ObservableCollection<Layer> _layers = new ObservableCollection<Layer>();
		private SKSurface _lossSurf, _networkSurf;
		private ChiruMatrix _lossData;

		public MainWindow()
		{
			InitializeComponent();
			_layers.Add(new Layer
			{
				LayerName = $"Layer 0",
				LayerDepth = 5
			});
			layers.ItemsSource = _layers;
			UpdateButtons();
			//Initialize Diagrams
			_lossSurf = SKSurface.Create(1680, 720, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);
			_networkSurf = SKSurface.Create(1680, 720, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);			
		}

		private void TrainXBrowse(object sender, RoutedEventArgs e) => Browse(d => trainXLocation.Dispatcher.Invoke(() => trainXLocation.Text = d.FileName));

		private void Browse(Action<System.Windows.Forms.OpenFileDialog> action)
		{
			using (var dialog = new System.Windows.Forms.OpenFileDialog())
			{
				dialog.Filter = "JSON | *.json";
				dialog.Multiselect = false;
				dialog.FileOk += (s, c) =>
				{
					action?.Invoke(dialog);
				};
				dialog.ShowDialog();
			}
		}

		private void TrainYBrowse(object sender, RoutedEventArgs e) => Browse(d => trainYLocation.Dispatcher.Invoke(() => trainYLocation.Text = d.FileName));

		private void TestXBrowse(object sender, RoutedEventArgs e) => Browse(d => testXLocation.Dispatcher.Invoke(() => testXLocation.Text = d.FileName));

		private void TestYBrowse(object sender, RoutedEventArgs e) => Browse(d => testYLocation.Dispatcher.Invoke(() => testYLocation.Text = d.FileName));

		private async void LoadTrainDataClick(object sender, RoutedEventArgs e)
		{
			string xL = trainXLocation.Text, yL = trainYLocation.Text;
			if (string.IsNullOrWhiteSpace(xL) || string.IsNullOrWhiteSpace(yL))
				return;
			(_trainX, _trainY) = await Task.Run(() => LoadData(xL, yL)); 
			progressBar.Value = 100;
			statusText.Content = StandbyMessage;
			_hasData = true;
			trainingSetDisplay.ItemsSource = await UpdateDataDisplay(_trainX, _trainY);
			UpdateButtons();
		}

		private async void LoadTestDataClick(object sender, RoutedEventArgs e)
		{
			string xL = testXLocation.Text, yL = testYLocation.Text;
			if (string.IsNullOrWhiteSpace(xL) || string.IsNullOrWhiteSpace(yL))
				return;
			(_testX, _testY) = await Task.Run(() => LoadData(xL, yL));
			progressBar.Value = 100;
			statusText.Content = StandbyMessage;
			_hasData = true;
			testingSetDisplay.ItemsSource = await UpdateDataDisplay(_testX, _testY, true);
			UpdateButtons();
			dataSetShape.Content = _testX.Shape.ToString();
		}

		private (ChiruMatrix X, ChiruMatrix Y) LoadData(string xL, string yL)
		{
			statusText.Dispatcher.Invoke(() => statusText.Content = "Loading Data: Training X...");
			progressBar.Dispatcher.Invoke(() => progressBar.Value = 0);
			var X = ChiruMatrix.FromJSON(File.ReadAllText(xL));
			progressBar.Dispatcher.Invoke(() => progressBar.Value = 50);
			statusText.Dispatcher.Invoke(() => statusText.Content = "Loading Data: Training Y...");
			var Y = ChiruMatrix.FromJSON(File.ReadAllText(yL));
			return (X, Y);
		}

		private async Task<ObservableCollection<ImageBox>> UpdateDataDisplay(ChiruMatrix x, ChiruMatrix y, bool showPredictResult = false)
		{
			inputSizeLabel.Content = $"Input Layer: {x.Height}";
			outputSizeLabel.Content = $"Ouput Layer: {y.Height}";
			var boxes = new ObservableCollection<ImageBox>();
			await Task.Run(() =>
			{
				using (MemoryStream s = new MemoryStream())
				{
					for (int i = 0; i < x.Width; i++)
					{
						statusText.Dispatcher.Invoke(() => statusText.Content = $"Decoding Image [{i+1}/{x.Width}]" );
						progressBar.Dispatcher.Invoke(() => progressBar.Value = ((double)i / (double)x.Width) * 100);
						ImagePreProcessor.Expand(x[i], s);
						var box = statusText.Dispatcher.Invoke(() =>
						{
							var b = new ImageBox
							{
								ImageName = $"IMG {i} - {y[i]}",
								ImageSource = new BitmapImage()
							};
							if(showPredictResult)
							{
								b.ImageName = $"{b.ImageName}:{(_predictY.IsEmpty() ? "[-]" : _predictY[i].ToString())}";
							}
							b.ImageSource.BeginInit();
							b.ImageSource.StreamSource = s;
							b.ImageSource.CacheOption = BitmapCacheOption.OnLoad;
							b.ImageSource.EndInit();
							return b;
						});
						boxes.Add(box);
						s.Position = 0;
						s.SetLength(0);
					}
				}
			});
			statusText.Content = StandbyMessage;
			progressBar.Value = 100;
			return boxes;
		}

		private int[] GetLayersArray()
		{
			int[] l = new int[_layers.Count];
			for (int i = 0; i < _layers.Count; i++)
			{
				l[i] = _layers[i].LayerDepth;
			}
			return l;
		}

		private ActivationFunction[] GetActivations()
		{
			var act = new ActivationFunction[_layers.Count + 2];
			Func<int, ActivationFunction> getFunc = (i) =>
			{
				switch(i)
				{
					case 0:
						return ActivationFunction.ReLu;
					case 1:
						return ActivationFunction.TanH;
					case 2:
						return ActivationFunction.Sigmoid;
					default:
						return ActivationFunction.TanH;
				}
			};
			act[0] = getFunc(inputActivation.SelectedIndex);
			act[act.Length-1] = getFunc(outputActivation.SelectedIndex);
			for (int i = 0; i < _layers.Count; i++)
			{
				act[i + 1] = getFunc(_layers[i].ActivationFunc);
			}
			return act;
		}

		private async void TrainOnce(object sender, RoutedEventArgs e)
		{
			if (_isRunning)
				return;
			if (_trainX.IsEmpty() || _trainY.IsEmpty())
				return;
			_isRunning = true;
			UpdateButtons();
			statusText.Content = "Training 1 Iteration...";
			double lRate = double.Parse(learningRate.Text);
			var l = GetLayersArray();
			var a = GetActivations();
			_parameters = await Task.Run(() => DeepNeuralNetwork.Model(_trainX / 255, _trainY, l, a, lRate, 1, _parameters, Status));
			statusText.Content = StandbyMessage;
			progressBar.Value = 100;
			_isRunning = false;
			UpdateButtons();
		}

		private async void TrainAll(object sender, RoutedEventArgs e)
		{
			if (_isRunning)
				return;
			if (_trainX.IsEmpty() || _trainY.IsEmpty())
				return;
			_isRunning = true;
			UpdateButtons();
			statusText.Content = $"Training {iterations.Text} Iterations...";
			double lRate = double.Parse(learningRate.Text);
			int it = int.Parse(iterations.Text);
			var l = GetLayersArray();
			var a = GetActivations();
			_parameters = await Task.Run(() => DeepNeuralNetwork.Model(_trainX / 255, _trainY, l, a, lRate, it, _parameters, Status, WillCancel));
			statusText.Content = StandbyMessage;
			progressBar.Value = 100;
			_isRunning = false;
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			trainOnce.IsEnabled = !_isRunning && _hasData && _layers.Count > 0;
			trainAll.IsEnabled = !_isRunning && _hasData && _layers.Count > 0;
			iterations.IsEnabled = !_isRunning && _hasData && _layers.Count > 0;
			learningRate.IsEnabled = !_isRunning && _hasData && _layers.Count > 0;
			stopTraining.IsEnabled = _isRunning;
			trainingDataGroup.IsEnabled = !_isRunning;
			networkShapeGroup.IsEnabled = !_isRunning;
			networkGroup.IsEnabled = !_isRunning;
			hyperparameterGroup.IsEnabled = !_isRunning;
			predictionGroup.IsEnabled = !(_parameters == null);
			removeLayer.IsEnabled = !_isRunning;
		}

		private void StopTraining(object sender, RoutedEventArgs e)
		{
			_willStop = true;
			statusText.Content = "Stopping...";
		}

		private bool WillCancel()
		{
			if(_willStop)
			{
				_willStop = false;
				return true;
			}
			return false;
		}

		private async void Status(int iteration, double cost)
		{
			statusText.Dispatcher.Invoke(() => statusText.Content = $"Training {iteration}/{iterations.Text} | Cost:{cost}");
			progressBar.Dispatcher.Invoke(() =>progressBar.Value = (iteration / double.Parse(iterations.Text)) * 100);
			_lossData = _lossData.IsEmpty() ? new double[,] { { cost } }.AsMatrix() : _lossData.AppendColumns(new double[,] { { cost } }.AsMatrix());
			await Task.Run(() => Graphs.DrawGraph(_lossSurf, _lossData, SKPMColor.PreMultiply(new SKColor(255, 0, 100))));
			var s = new MemoryStream();
			_lossSurf.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo(s);
			lossGraph.Dispatcher.Invoke(() =>
			{
				var b = new BitmapImage();
				b.BeginInit();
				b.StreamSource = s;
				b.CacheOption = BitmapCacheOption.OnLoad;
				b.EndInit();
				lossGraph.Source = b;
			});
		}

		private void ValidateText(object sender, System.Windows.Input.KeyEventArgs e)
		{
			var tb = (TextBox)sender;
			if (!double.TryParse(tb.Text, out double value))
				tb.Text = "1";
		}

		private void LoadNetwork(object sender, RoutedEventArgs e)
		{
			using (var open = new System.Windows.Forms.OpenFileDialog())
			{
				open.Filter = "JSON | *.json";
				open.DefaultExt = "json";
				open.Multiselect = false;
				open.FileOk += (s, c) =>
				{
					_parameters = Parameters.FromJSON(File.ReadAllText(open.FileName));
				};
				open.ShowDialog();
			}
			if (_parameters == null)
				return;
			_layers.Clear();
			for (int i = 0; i < _parameters.B.Length - 1; i++)
			{
				_layers.Add(new Layer
				{
					LayerDepth = _parameters.B[i].Height
				});
			}
			_lossData = default;
			UpdateLayerNames();
			UpdateButtons();
		}

		private  void SaveNetwork(object sender, RoutedEventArgs e)
		{
			using (var save = new System.Windows.Forms.SaveFileDialog())
			{
				save.Filter = "JSON | *.json";
				save.DefaultExt = "json";
				save.FileOk += (s, c) =>
				{
					File.WriteAllText(save.FileName, _parameters.ToJSON());
				};
				save.ShowDialog();
			}
		}

		private void ResetNetwork(object sender, RoutedEventArgs e)
		{
			_parameters = null;
			_layers.Clear();
			_layers.Add(new Layer
			{
				LayerDepth = 5
			});
			_lossData = default;
			UpdateLayerNames();
			UpdateButtons();
		}

		private async void PredictSelected(object sender, RoutedEventArgs e)
		{
			var a = GetActivations();
			var p = testingSetDisplay.SelectedIndex;
			if (_predictY.IsEmpty())
				_predictY = ChiruMatrix.Zeros(_testY.Height, _testY.Width);
			_predictY[p] = await Task.Run(() => DeepNeuralNetwork.Predict(_parameters, _testX[p], a));
		}

		private async void PredictAll(object sender, RoutedEventArgs e)
		{
			var a = GetActivations();
			_predictY = await Task.Run(() => DeepNeuralNetwork.Predict(_parameters, _testX, a));
			UpdatePredictStats();
		}

		private void UpdatePredictStats()
		{
			predictAccuracy.Content = _predictY.IsEmpty() ? "No Data" :  _predictY.ErrorWith(_testY).ToString();
		}

		private void AddLayer(object sender, RoutedEventArgs e)
		{
			_layers.Add(new Layer
			{
				LayerDepth = 5
			});
			UpdateButtons();
			UpdateLayerNames();
		}

		private void RemoveLayer(object sender, RoutedEventArgs e)
		{
			if(layers.SelectedIndex >= 0)
				_layers.RemoveAt(layers.SelectedIndex);
			UpdateButtons();
			UpdateLayerNames();
		}

		private void UpdateLayerNames()
		{
			for (int i = 0; i < _layers.Count; i++)
			{
				_layers[i].LayerName = $"Layer {i}";
			}
			layers.UpdateLayout();
		}
	}
}
