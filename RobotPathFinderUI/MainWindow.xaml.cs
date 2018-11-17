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
using RobotPathFinder;
using static System.Int32;

namespace RobotPathFinderUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

		private int sizeX, sizeY;
		private int horizontalCost, verticalCost, diagonalCost;
		private static readonly Regex numberRegex = new Regex("[^0-9.]+");
		private IRobotGrid robotGrid;
		private NodeLabel star, end;
		private List<NodeLabel> nodeLabels = new List<NodeLabel>();
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
			horizontalCost = Parse(txtHorizontalCost.Text);
			verticalCost = Parse(txtVerticalCost.Text);
			diagonalCost = Parse(txtDiagonalCost.Text);
			GenerateNodeGrid();

		}

		private void GenerateNodeGrid() {
			ClearGrid();
			if (sizeX == 0 || sizeY == 0) throw new Exception("Grid size is not initialized.");
			var nodePosition = new NodePosition {X = -450, Y = -350, AdditionalData = "LabelPosition"};
			robotGrid = new RobotGrid(sizeX, sizeY, horizontalCost, verticalCost, diagonalCost);

			for (int i = 0; i < sizeY; i++) {
				for (int j = 0; j < sizeX; j++) {
					CreateNode(robotGrid.AllNodes.GetValue(i, j) as Node, nodePosition);
					 nodePosition.X += 55;
				}
				nodePosition.X = -450;
				nodePosition.Y += 55;
			}
		}

		private void btnFindPath_Click(object sender, RoutedEventArgs e) {
			if (!robotGrid.IsInitialized) {
				MessageBox.Show("Grid is not initialized.");
				return;
			}

			if (star == null || end == null) {
				MessageBox.Show("Start or End is not set.");
				return;
			}

			try {
				var result = robotGrid.FindPath(star.Id, end.Id);
				if (result.Any()) DrawPath(result);
			} catch (Exception exception) {
				Console.WriteLine(exception);
				MessageBox.Show(exception.Message);
			}
		}

		private void DrawPath(List<Node> result) {
			result.ForEach(x => {
				nodeLabels.First(y=> y.Id == x.Id).Background = new SolidColorBrush(Colors.GreenYellow);
			});
		}

		private void CreateNode(Node node, NodePosition place) {
			var nodeLabel = new NodeLabel {
				Name = $"lbl{node.Position.X}i{node.Position.Y}",
				Id = node.Id,
				Position = node.Position,
				Visibility = Visibility.Visible,
				Width = 25,
				Height = 25,
				Background = new SolidColorBrush(Colors.Gray),
				Margin = new Thickness(place.X, place.Y, 0, 0)
			};
			nodeLabel.MouseLeftButtonUp += NodeClick;
			nodeGrid.Children.Add(nodeLabel);
			nodeLabels.Add(nodeLabel);
		}

		private void NodeClick(object sender, MouseButtonEventArgs e) {
			var node = sender as NodeLabel;
			if (node == null || !robotGrid.IsInitialized) return;
			if (btnSetBlock.IsChecked ?? false) {
				robotGrid.SetObstacles(node.Id);
				node.Background = new SolidColorBrush(Colors.Red);
			}
			if (btnSetStart.IsChecked ?? false) {
				star = node;
				node.Background = new SolidColorBrush(Colors.Green);
			} else if (btnSetEnd.IsChecked ?? false) {
				end = node;
				node.Background = new SolidColorBrush(Colors.Blue);
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

		private void ClearGrid() {
			nodeGrid.Children.Clear();
			star = null;
			end = null;
			robotGrid = null;
			nodeLabels.Clear();
		}


	}

	public class NodeLabel : Label {

		public NodePosition Position { get; set; }
		public int Id { get; set; }

	}
}
