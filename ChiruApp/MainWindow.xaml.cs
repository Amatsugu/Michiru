using Michiru.Calculation;
using Michiru.Regression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChiruApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var trainX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\X.json"));
			var trainY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\Y.json"));
			var testY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\Y.json"));
			var testX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\X.json"));
			


			//Standardize
			trainX /= 255;
			testX /= 255;

			Thread t = new Thread(() =>
			{

				var result = LogisticRegression.Model(trainX, trainY, testX, testY, 500, .005, true, x => { });

				File.WriteAllText(@"D:\ChiruData\modelW.json", result.w.ToJSON());
				File.WriteAllText(@"D:\ChiruData\modelB.json", result.b.ToString());
			});
		}
	}
}
