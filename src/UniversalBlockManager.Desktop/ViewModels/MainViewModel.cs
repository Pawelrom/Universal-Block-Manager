using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using UniversalBlockManager.Shared.Models;

namespace UniversalBlockManager.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _blockName = "NewBlock";
        private string _units = "mm";
        private double _basePointX = 0;
        private double _basePointY = 0;
        private double _scaleFactor = 1.0;

        public string BlockName { get => _blockName; set => SetProperty(ref _blockName, value); }
        public string Units { get => _units; set => SetProperty(ref _units, value); }
        public double BasePointX { get => _basePointX; set => SetProperty(ref _basePointX, value); }
        public double BasePointY { get => _basePointY; set => SetProperty(ref _basePointY, value); }
        public double ScaleFactor { get => _scaleFactor; set => SetProperty(ref _scaleFactor, value); }

        public ObservableCollection<BlockAttribute> Attributes { get; } = new ObservableCollection<BlockAttribute>();

        public ICommand SaveCommand { get; }
        public ICommand AddAttributeCommand { get; }

        public MainViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
            AddAttributeCommand = new RelayCommand(_ => Attributes.Add(new BlockAttribute { Key = "Key", Value = "Value" }));

            // Default attribute
            Attributes.Add(new BlockAttribute { Key = "Producent", Value = "Unknown" });
        }

        private void Save()
        {
            var sfd = new SaveFileDialog { Filter = "XML Files (*.xml)|*.xml", FileName = $"{BlockName}.xml" };
            if (sfd.ShowDialog() == true)
            {
                var block = new BlockDefinition
                {
                    Name = BlockName,
                    Units = Units,
                    Header = new Header
                    {
                        BasePoint = new Point { X = BasePointX, Y = BasePointY },
                        ScaleFactor = ScaleFactor
                    },
                    Attributes = Attributes.ToList()
                };

                block.Save(sfd.FileName);
            }
        }
    }
}
