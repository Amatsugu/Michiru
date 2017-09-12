using Michiru.Calculation;
using Michiru.Neural;
using Utils;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChiruApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ChiruMatrix _trainX;
		private ChiruMatrix _trainY;
		private Parameters _parameters = null;
		private bool _willStop = false;
		private bool _isRunning = false;
		private const string StandbyMessage = "Ready!";
		private ObservableCollection<Layer> _layers = new ObservableCollection<Layer>();
		public MainWindow()
		{
			InitializeComponent();
			_layers.Add(new Layer
			{
				LayerName = $"Layer 0",
				LayerDepth = 5
			});
			layers.ItemsSource = _layers;
		}

		private void XBrowse(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.OpenFileDialog())
			{
				dialog.FileOk += (s, c) =>
				{
					xLocation.Text = dialog.FileName;
				};
				dialog.ShowDialog();
			}
		}

		private void YBrowse(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.OpenFileDialog())
			{
				dialog.FileOk += (s, c) =>
				{
					yLocation.Text = dialog.FileName;
				};
				dialog.ShowDialog();
			}
		}

		private async void LoadDataClick(object sender, RoutedEventArgs e)
		{
			string xL = xLocation.Text, yL = yLocation.Text;
			statusText.Content = "Loading Data: Training X,Y...";
			progressBar.Value = 50;
			
			(_trainX, _trainY) = await Task.Run(() =>
			{
				var X = ChiruMatrix.FromJSON(File.ReadAllText(xL));
				var Y = ChiruMatrix.FromJSON(File.ReadAllText(yL));
				return (X, Y);
			}); 
			progressBar.Value = 100;
			statusText.Content = StandbyMessage;
			UpdateDataDisplay();
		}

		private void UpdateDataDisplay()
		{
			inputSizeLabel.Content = $"Input Layer: {_trainX.Height}";
			outputSizeLabel.Content = $"Ouput Layer: {_trainY.Height}";
			var boxes = new ObservableCollection<ImageBox>();
			using (MemoryStream s = new MemoryStream())
			{
				for (int i = 0; i < _trainX.Width; i++)
				{
					statusText.Content = $"Decoding Image [{i+1}/{_trainX.Width}]";
					ImagePreProcessor.Expand(_trainX[i], s);
					var box = new ImageBox
					{
						ImageName = $"IMG {i} - {_trainY[i]}",
						ImageSource = new BitmapImage()
					};
					box.ImageSource.BeginInit();
					box.ImageSource.StreamSource = s;
					box.ImageSource.CacheOption = BitmapCacheOption.OnLoad;
					box.ImageSource.EndInit();
					boxes.Add(box);
					s.Position = 0;
					s.SetLength(0);
					progressBar.Value = (i / _trainX.Width) * 100;
				}
			}
			statusText.Content = StandbyMessage;
			progressBar.Value = 100;
			trainingSetDisplay.ItemsSource = boxes;
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

		private async void TrainOnce(object sender, RoutedEventArgs e)
		{
			if (_isRunning)
				return;
			if (_trainX.IsEmpty() || _trainY.IsEmpty())
				return;
			_isRunning = true;
			UpdateTrainingButtons();
			statusText.Content = "Training 1 Iteration...";
			_parameters = await Task.Run(() => DeepNeuralNetwork.Model(_trainX, _trainY, GetLayersArray(), 0.0075, 1, false, Status, _parameters));
			statusText.Content = StandbyMessage;
			_isRunning = false;
			UpdateTrainingButtons();
		}

		private async void TrainAll(object sender, RoutedEventArgs e)
		{
			if (_isRunning)
				return;
			if (_trainX.IsEmpty() || _trainY.IsEmpty())
				return;
			_isRunning = true;
			UpdateTrainingButtons();
			statusText.Content = $"Training {iterations.Text} Iterations...";
			_parameters = await Task.Run(() => DeepNeuralNetwork.Model(_trainX, _trainY, GetLayersArray(), 0.0075, int.Parse(iterations.Text), false, Status, _parameters, WillCancel));
			statusText.Content = StandbyMessage;
			_isRunning = false;
			UpdateTrainingButtons();
		}

		private void UpdateTrainingButtons()
		{
			trainOnce.IsEnabled = !_isRunning;
			trainAll.IsEnabled = !_isRunning;
			stopTraining.IsEnabled = _isRunning;
			addLayer.IsEnabled = !_isRunning;
			removeLayer.IsEnabled = !_isRunning;
		}

		private void StopTraining(object sender, RoutedEventArgs e)
		{
			_willStop = true;
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

		private void Status(int iteration, double cost)
		{

		}

		private void ValidateText(object sender, System.Windows.Input.KeyEventArgs e)
		{
			var tb = (TextBox)sender;
			if (!double.TryParse(tb.Text, out double value))
				tb.Text = "1";
		}

		private void LoadNetwork(object sender, RoutedEventArgs e)
		{

		}

		private void SaveNetwork(object sender, RoutedEventArgs e)
		{

		}

		private void ResetNetwork(object sender, RoutedEventArgs e)
		{
			if (_parameters == null)
				return;
			_parameters = new Parameters(2);
		}

		private void AddLayer(object sender, RoutedEventArgs e)
		{
			_layers.Add(new Layer
			{
				LayerName = $"Layer {_layers.Count}",
				LayerDepth = 5
			});
		}

		private void RemoveLayer(object sender, RoutedEventArgs e)
		{
			if(layers.SelectedIndex >= 0)
				_layers.RemoveAt(layers.SelectedIndex);
		}
	}
}
