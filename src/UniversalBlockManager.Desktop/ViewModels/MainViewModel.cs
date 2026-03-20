using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using UniversalBlockManager.Shared.Models;
using UniversalBlockManager.Shared.Services;
using SkiaSharp;

namespace UniversalBlockManager.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly VectorizationService _vectorizationService;
        private readonly BlockDefinition _block;

        public MainViewModel()
        {
            _vectorizationService = new VectorizationService();
            _block = new BlockDefinition
            {
                Name = "NewBlock",
                Units = "mm",
                Header = new Header { BasePoint = new Point { X = 0, Y = 0 }, ScaleFactor = 1.0 },
                Attributes = new List<BlockAttribute>(),
                Layers = new List<Layer>()
            };

            Attributes = new ObservableCollection<BlockAttribute>();
            AddAttributeCommand = new RelayCommand(_ => AddAttribute());
            SaveCommand = new RelayCommand(_ => SaveBlock());
            LoadImageCommand = new RelayCommand(_ => LoadImage());
            
            _imagePath = string.Empty;
            _svgPathData = string.Empty;
            _imageWidth = 500;
            _imageHeight = 500;
        }

        #region Properties
        public string BlockName
        {
            get => _block.Name ?? string.Empty;
            set { _block.Name = value; OnPropertyChanged(); }
        }

        public string Units
        {
            get => _block.Units ?? string.Empty;
            set { _block.Units = value; OnPropertyChanged(); }
        }

        public double BasePointX
        {
            get => _block.Header.BasePoint.X;
            set { _block.Header.BasePoint.X = value; OnPropertyChanged(); }
        }

        public double BasePointY
        {
            get => _block.Header.BasePoint.Y;
            set { _block.Header.BasePoint.Y = value; OnPropertyChanged(); }
        }

        public double ScaleFactor
        {
            get => _block.Header.ScaleFactor;
            set { _block.Header.ScaleFactor = value; OnPropertyChanged(); }
        }

        public ObservableCollection<BlockAttribute> Attributes { get; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        private string _svgPathData;
        public string SvgPathData
        {
            get => _svgPathData;
            set { _svgPathData = value; OnPropertyChanged(); }
        }

        private double _imageWidth;
        public double ImageWidth
        {
            get => _imageWidth;
            set { _imageWidth = value; OnPropertyChanged(); }
        }

        private double _imageHeight;
        public double ImageHeight
        {
            get => _imageHeight;
            set { _imageHeight = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand AddAttributeCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadImageCommand { get; }
        #endregion

        private void AddAttribute()
        {
            var attr = new BlockAttribute { Key = "NewKey", Value = "NewValue" };
            Attributes.Add(attr);
            _block.Attributes.Add(attr);
        }

        private void SaveBlock()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "XML files (*.xml)|*.xml",
                FileName = $"{BlockName}.xml"
            };

            if (dialog.ShowDialog() == true)
            {
                _block.Save(dialog.FileName);
            }
        }

        private void LoadImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePath = dialog.FileName;
                UpdateImageInfoAndTrace();
            }
        }

        private void UpdateImageInfoAndTrace()
        {
            if (string.IsNullOrEmpty(ImagePath)) return;

            try
            {
                // Get Image Info using SkiaSharp to update Canvas size
                using (var stream = File.OpenRead(ImagePath))
                using (var bitmap = SKBitmap.Decode(stream))
                {
                    ImageWidth = bitmap.Width;
                    ImageHeight = bitmap.Height;
                }

                // Trace
                SvgPathData = _vectorizationService.TraceImageToSvgPath(ImagePath);
                
                _block.Geometry.Clear();
                _block.Geometry.Add(new PathElement { Data = SvgPathData, Layer = "0" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image load error: {ex.Message}");
            }
        }
    }
}
