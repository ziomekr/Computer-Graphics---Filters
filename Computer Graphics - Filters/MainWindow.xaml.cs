using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConvolutionFilter activeFilter;
        private Button activeButton;
        private List<TextBox> KernelFields = new List<TextBox>();
        private List<ConvolutionFilter> ConvolutionFilters = new List<ConvolutionFilter>();
        private int FiltersCount = 6;
        private int anchorX, anchorY, kernelX, kernelY;
        public MainWindow()
        {
            InitializeComponent();
            InitializeFilters();
        }

        //Select image and load
        private void LoadImageClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select an image to load";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                BitmapImage toLoad = LoadImage(op.FileName);
                image.Source = toLoad;
                originalImage.Source = toLoad;
                ResetShowOriginalImage.Visibility = Visibility.Visible;
            }
        }
        //Reset image to original state
        private void ResetImageClick(object sender, RoutedEventArgs e)
        {
            image.Source = originalImage.Source;
        }
        private void ShowOriginalCheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                columnOriginal.Width = new GridLength(1, GridUnitType.Star);
                OriginalImageView.Visibility = Visibility.Visible;
            }
            else
            {
                columnOriginal.Width = new GridLength(0, GridUnitType.Pixel);
                OriginalImageView.Visibility = Visibility.Collapsed;
            }
        }
            //Filters initialization
            private void InitializeFilters() {
            BitmapSource img = (BitmapSource)image.Source;
            ConvolutionFilters.Add(new BlurFilter(img));
            ConvolutionFilters.Add(new GaussianSmootheningFilter(img));
            ConvolutionFilters.Add(new SharpenFilter(img));
            ConvolutionFilters.Add(new EdgeDetectionFilter(img));
            ConvolutionFilters.Add(new EmbossFilter(img));
            ConvolutionFilters.Add(new MedianFilter(img));
        }

        //Apply inversion filter
        private void InvertionClick(object sender, RoutedEventArgs e) {
            if(image.Source!=null)
                image.Source = new FunctionalFilter((BitmapSource)image.Source).Inversion();
        }

        //Apply brightness correction filter
        private void BrightnessClick(object sender, RoutedEventArgs e)
        {
            if (image.Source != null)
            {
                image.Source = new FunctionalFilter((BitmapSource)image.Source).BrightnessCorrection((int)BrightnessSlider.Value);
            }
        }

        //Apply contrast enchancement filter
        private void ContrastClick(object sender, RoutedEventArgs e)
        {
            if (image.Source != null)
            {
                image.Source = new FunctionalFilter((BitmapSource)image.Source).ContrastEnhancement(ContrastSlider.Value);
            }
        }
        //Apply gamma correction filter
        private void GammaClick(object sender, RoutedEventArgs e)
        {
            if (image.Source != null)
            {
                image.Source = new FunctionalFilter((BitmapSource)image.Source).GammaCorrection(GammaSlider.Value);
            }
        }
        //Select convolution filter
        private void ConvolutionFilterClick(object sender, RoutedEventArgs e)
        {
            if(activeButton!=null)
                activeButton.BorderBrush = System.Windows.Media.Brushes.Black;

            activeButton = (Button)sender;
            activeButton.BorderBrush = System.Windows.Media.Brushes.Red;
            ModifyFilter.IsChecked = false;
            int filter_index = 0;
            int.TryParse(activeButton.Tag.ToString(), out filter_index);
            activeFilter = ConvolutionFilters.ElementAt(filter_index);
            PopulateFilterInfo(activeFilter);
            
        }

        //Apply selected filter
        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            if (image.Source != null && activeFilter!=null)
            {
                activeFilter.ChangeImage((BitmapSource)image.Source);
                image.Source = activeFilter.FilterImage();
            }
        }
        //Apply modifications to filter
        private void ApplyChangesToFilterClick(object sender, RoutedEventArgs e)
        {
            int kernelX, kernelY, anchorX, anchorY, offset;
            double[,] newKernel;
            double divisor;

            int.TryParse(KernelX.Text.ToString(), out kernelX);
            int.TryParse(KernelY.Text.ToString(), out kernelY);
            int.TryParse(AnchorX.Text.ToString(), out anchorX);
            int.TryParse(AnchorY.Text.ToString(), out anchorY);
            int.TryParse(Offset.Text.ToString(), out offset);
            double.TryParse(Divisor.Text.ToString(), out divisor);
            newKernel = new double[kernelX, kernelY];

            for (int i = 0; i < kernelX; i++)
            {
                for (int j = 0; j < kernelY; j++)
                {
                    TextBox t = KernelFields.ElementAt(i * kernelX + j);
                    if (!double.TryParse(t.Text.ToString(), out newKernel[i, j]))
                        newKernel[i, j] = 0;
                }
            }

            activeFilter.Kernel = newKernel;
            activeFilter.AnchorX = anchorX;
            activeFilter.AnchorY = anchorY;
            activeFilter.Divisor = divisor;
            activeFilter.Offset = offset;
            ModifyFilter.IsChecked = false;
        }
        //New filter button click
        private void NewFilterClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.Visibility = Visibility.Collapsed;
            NewFilterWrapPanel.Visibility = Visibility.Visible;
        }
        //Add filter button click
        private void AddFilterClick(object sender, RoutedEventArgs e)
        {
            NewFilterButton.Visibility = Visibility.Visible;
            NewFilterWrapPanel.Visibility = Visibility.Collapsed;
            AddNewFilter(FilterName.Text);
            FilterName.Text = "";
            ModifyFilter.IsChecked = true;
        }
        //Modify filter checkbox checked change handler
        private void ModifyFilterCheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (activeButton != null && activeFilter != null)
            {
                if (checkBox.IsChecked == true)
                {
                    ApplyButton.Visibility = System.Windows.Visibility.Collapsed;
                    ApplyChangesToFilter.Visibility = System.Windows.Visibility.Visible;
                    EnableDisableFilterEdition();
                }
                else {
                    ApplyButton.Visibility = System.Windows.Visibility.Visible;
                    ApplyChangesToFilter.Visibility = System.Windows.Visibility.Collapsed;
                    EnableDisableFilterEdition();
                }
            }
            else {
                checkBox.IsChecked = false;
            }
        }
        //Enable or disable filter edition
        private void EnableDisableFilterEdition() {
            KernelGrid.IsEnabled = !KernelGrid.IsEnabled;
            KernelX.IsEnabled = !KernelX.IsEnabled;
            KernelY.IsEnabled = !KernelY.IsEnabled;
            AnchorX.IsEnabled = !AnchorX.IsEnabled;
            AnchorY.IsEnabled = !AnchorY.IsEnabled;
            Offset.IsEnabled = !Offset.IsEnabled;
            Divisor.IsEnabled = !Divisor.IsEnabled;
        }
        //Thresholding filter click
        private void ThresholdingClick(object sender, RoutedEventArgs e)
        {
            if (image.Source != null)
                image.Source = new ThresholdingFilter((BitmapSource)image.Source, ThresholdSlider.Value, (int)KSlider.Value).FilterImage();
        }

        //Open image as stream from file
        private BitmapImage LoadImage(string FileName) {
            MemoryStream ms = new MemoryStream();
            FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            ms.SetLength(stream.Length);
            stream.Read(ms.GetBuffer(), 0, (int)stream.Length);
            ms.Flush();
            stream.Close();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.StreamSource = ms;
            src.EndInit();
            return src;
        }

        //Fill controls with filter paramters
        private void PopulateFilterInfo(ConvolutionFilter filter) {
            RemoveKernelTextBoxes();
            AddKernelTextBoxes(filter.Kernel.GetLength(0), filter.Kernel.GetLength(1));
            KernelGrid.IsEnabled = false;
            KernelX.Text = filter.Kernel.GetLength(0).ToString();
            KernelX.IsEnabled = false;
            KernelY.Text = filter.Kernel.GetLength(1).ToString();
            KernelY.IsEnabled = false;
            AnchorX.Text = filter.AnchorX.ToString();
            AnchorX.IsEnabled = false;
            AnchorY.Text = filter.AnchorY.ToString();
            AnchorY.IsEnabled = false;
            Offset.Text = filter.Offset.ToString();
            Offset.IsEnabled = false;
            Divisor.Text = filter.Divisor.ToString();
            Divisor.IsEnabled = false;
            FillKernelValues(filter);
            TextBox t = KernelFields.ElementAt(filter.AnchorY*filter.Kernel.GetLength(0)+filter.AnchorX);
            t.BorderBrush = System.Windows.Media.Brushes.Red;
        }

        //Fill kernel grid fields with kernel values
        private void FillKernelValues(ConvolutionFilter filter) {
            for (int i = 0; i < filter.Kernel.GetLength(0); i++) {
                for (int j = 0; j < filter.Kernel.GetLength(1); j++)
                {
                    TextBox t = KernelFields.ElementAt(i*filter.Kernel.GetLength(1) + j);
                    t.Text = filter.Kernel[i, j].ToString();
                }
            }
        }

        //Add enough fields to kernel grid
        private void AddKernelTextBoxes(int x, int y) {
            KernelGrid.Columns = x;
            for (int i = 0; i < x * y; i++) {
                TextBox t = new TextBox();
                t.BorderBrush = System.Windows.Media.Brushes.Black;
                t.Name = "KernelField" + i.ToString();
                t.Height = 40;
                t.Width = 40;
                t.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                t.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                t.TextChanged += this.KernelFieldTextChanged;
                KernelGrid.Children.Add(t);
                KernelFields.Add(t);
            }
           
        }
        //Remove all kernel fields from the kernel grid
        private void RemoveKernelTextBoxes() {
            KernelFields.RemoveRange(0, KernelFields.Count);
            KernelGrid.Children.RemoveRange(0, KernelGrid.Children.Count);
        }

        //Check if kernel field input is valid, double values only
        private void KernelFieldTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (string.IsNullOrEmpty(t.Text))
                t.Tag = "";
            else
            {
                if (t.Text == "-")
                    return;
                double num = 0;
                t.Text = Regex.Replace(t.Text, "\\.", ",");
                t.SelectionStart = t.Text.Length;
                if (double.TryParse(t.Text, out num))
                {
                    t.Text = t.Text.Trim();
                    t.Tag = t.Text;
                }
                else
                {
                    t.Text = (string)t.Tag;
                    t.SelectionStart = t.Text.Length;
                }
            }
        }
        //Update kernel grid to right size, checking for positive integer
        private void KernelSizeTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (string.IsNullOrEmpty(t.Text))
                t.Tag = "";
            else
            {
                int num = 0;
               
                if (int.TryParse(t.Text, out num) && num>0)
                {
                    t.Text = t.Text.Trim();
                    t.Tag = t.Text;
                    int x, y;
                    int.TryParse(KernelX.Text.ToString(), out x);
                    int.TryParse(KernelY.Text.ToString(), out y);
                    RemoveKernelTextBoxes();
                    AddKernelTextBoxes(x,y);
                    kernelX = x;
                    kernelY = y;
                }
                else
                {
                    t.Text = (string)t.Tag;
                    t.SelectionStart = t.Text.Length;
                }
            }
        }

        //Update anchor highlight, checking for integer >=0
        private void AnchorTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (string.IsNullOrEmpty(t.Text))
                t.Tag = "";
            else
            {
                int num = 0, max;
                if (t.Name == "AnchorX")
                {
                    max = kernelX;
                }
                else {
                    max = kernelY;
                }
                if (int.TryParse(t.Text, out num) && num < max)
                {
                    t.Text = t.Text.Trim();
                    t.Tag = t.Text;
                    int x, y;
                    int.TryParse(AnchorX.Text.ToString(), out x);
                    int.TryParse(AnchorY.Text.ToString(), out y);
                    try
                    {
                        t = KernelFields.ElementAt(anchorY * kernelX + anchorX);
                        t.BorderBrush = System.Windows.Media.Brushes.Black;
                        t = KernelFields.ElementAt(y * kernelX + x);
                        t.BorderBrush = System.Windows.Media.Brushes.Red;
                    }
                    catch { }
                    anchorX = x;
                    anchorY = y;
                }
                else
                {
                    t.Text = (string)t.Tag;
                    t.SelectionStart = t.Text.Length;
                }
            }
        }


        //Check if offset input is valid, integer values only
        private void OffsetTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (string.IsNullOrEmpty(t.Text))
                t.Tag = "";
            else
            {
                if (t.Text == "-")
                    return;
                int num = 0;
                
                if (int.TryParse(t.Text, out num))
                {
                    t.Text = t.Text.Trim();
                    t.Tag = t.Text;
                }
                else
                {
                    t.Text = (string)t.Tag;
                    t.SelectionStart = t.Text.Length;
                }
            }
        }


        //Check if divisor input is valid, positive double values only
        private void DivisorTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (string.IsNullOrEmpty(t.Text))
                t.Tag = "";
            else
            {
                double num = 0;
                t.Text = Regex.Replace(t.Text, "\\.", ",");
                t.SelectionStart = t.Text.Length;
                if (double.TryParse(t.Text, out num) && num>0)
                {
                    t.Text = t.Text.Trim();
                    t.Tag = t.Text;
                }
                else
                {
                    t.Text = (string)t.Tag;
                    t.SelectionStart = t.Text.Length;
                }
            }
        }

        //Add new filter to the filters list
        private void AddNewFilter(string Name) {
            //Add button
            Button filterButton = new Button();
            filterButton.Margin = new Thickness(5);
            filterButton.Width = 150;
            filterButton.Tag = FiltersCount++.ToString();
            filterButton.Content = Name;
            filterButton.Click += ConvolutionFilterClick;
            ConvolutionFiltersPanel.Children.Add(filterButton);
            //Add empty filter
            ConvolutionFilters.Add(new ConvolutionFilter(null, new double[1, 1], 0, 0, 0, 1));
            ConvolutionFilterClick(filterButton, null);
        }
    }

    
}
