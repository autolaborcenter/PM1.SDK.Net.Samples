using System.Windows;

namespace Autolabor.PM1.TestTool.MainWindowItems.CalibrationTab {
    internal class CalculateContext : BindableBase {
        private bool _inverse;
        private string _unit;
        private double _actual, _odometry, _current;

        public bool Inverse {
            get => _inverse;
            set {
                if (!SetProperty(ref _inverse, value)) return;
                Notify(nameof(Sign0));
                Notify(nameof(Sign1));
            }
        }

        public string Sign0 => _inverse ? "÷" : "×";

        public string Sign1 => _inverse ? "×" : "÷";

        public string Unit {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        public double Actual {
            get => _actual;
            set {
                if (SetProperty(ref _actual, value))
                    Notify(nameof(Calculated));
            }
        }

        public double Odometry {
            get => _odometry;
            set {
                if (SetProperty(ref _odometry, value)) {
                    _actual = _odometry;
                    Notify(nameof(Actual));
                    Notify(nameof(Calculated));
                }
            }
        }

        public double Current {
            get => _current;
            set {
                if (SetProperty(ref _current, value))
                    Notify(nameof(Calculated));
            }
        }

        public double Calculated
            => _inverse
            ? _current / (_actual / _odometry)
            : _current * (_actual / _odometry);
    }

    /// <summary>
    /// CalculateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CalculateWindow : Window {
        private CalculateContext _context;

        public CalculateWindow(double odometry, bool inverse, string unit, double current) {
            _context = new CalculateContext {
                Odometry = odometry,
                Inverse = inverse,
                Unit = unit,
                Current = current
            };

            InitializeComponent();
        }

        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            var @new = (CalculateContext)e.NewValue;
            @new.Odometry = _context.Odometry;
            @new.Current = _context.Current;
            @new.Unit = _context.Unit;
            @new.Inverse = _context.Inverse;
            _context = @new;
        }
    }
}
