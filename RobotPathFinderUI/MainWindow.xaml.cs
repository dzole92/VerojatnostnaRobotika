using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Int32;

namespace RobotPathFinderUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

		private int sizeX, sizeY;
		private double horizontalCost, verticalCost, diagonalCost;
		private static readonly Regex numberRegex = new Regex("[^0-9.]+");
		public MainWindow()
        {
            InitializeComponent();
        }

		private void btnGenerateGrid_click(object sender, RoutedEventArgs e) {
			if (!ValidateInputs()) {
				MessageBox.Show("Insert right values. Numbers only");
				return;
			}

			sizeX = Parse(txtSizeX.Text);
			sizeY = Parse(txtSizeY.Text);
			GenerateNodeGrid();

		}

		private void GenerateNodeGrid() {
			if(sizeX == 0 || sizeY == 0) throw new Exception("Grid size is not initialized.");
			for (int i = 0; i < sizeY; i++) {
				for (int j = 0; j < sizeX; j++) {
					
				}
			}
		}

		private bool ValidateInputs() => !string.IsNullOrEmpty(txtSizeX.Text) 
										&& !string.IsNullOrEmpty(txtSizeY.Text) 
										&& !string.IsNullOrEmpty(txtHorizontalCost.Text) 
										&& !string.IsNullOrEmpty(txtDiagonalCost.Text) 
										&& !string.IsNullOrEmpty(txtVerticalCost.Text) 
										&& !numberRegex.IsMatch(txtSizeX.Text)
										&& !numberRegex.IsMatch(txtHorizontalCost.Text)
										&& !numberRegex.IsMatch(txtDiagonalCost.Text)
										&& !numberRegex.IsMatch(txtVerticalCost.Text)
										&& !numberRegex.IsMatch(txtSizeY.Text);


	}
}
