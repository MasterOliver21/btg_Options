using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BTGOpcoes.ViewModels
{
    public class OptionsCalcViewModel : BaseViewModel
    {
        private int? _opcaoSelecionada;
        private double _precoAcao;
        private double _precoStrike;
        private double _volatilidadePorcentagem;
        private double _jurosPorcentagem;
        private double _tempoDias;
        private double? _resultado;
        private double? _resultadoDelta;
        private double? _resultadoGamma;
        private double? _resultadoTheta;
        private double? _resultadoVega;
        private double? _resultadoRho;

        public ICommand CalcularCommand { get; private set; }
        public ICommand LimparCommand { get; private set; }

        public OptionsCalcViewModel()
        {
            CalcularCommand = new Command(execute: () => Calcular());
            LimparCommand = new Command(execute: () => Limpar());
        }

        public int? OpcaoSelecionada
        {
            get => _opcaoSelecionada;
            set
            {
                _opcaoSelecionada = value;
                OnPropertyChanged();
            }
        }

        public double PrecoAcao
        {
            get => _precoAcao;
            set
            {
                _precoAcao = value;
                OnPropertyChanged();
            }
        }

        public double PrecoStrike
        {
            get => _precoStrike;
            set
            {
                _precoStrike = value;
                OnPropertyChanged();
            }
        }

        public double VolatilidadePorcentagem
        {
            get => _volatilidadePorcentagem;
            set
            {
                _volatilidadePorcentagem = value;
                OnPropertyChanged();
            }
        }

        public double JurosPorcentagem
        {
            get => _jurosPorcentagem;
            set
            {
                _jurosPorcentagem = value;
                OnPropertyChanged();
            }
        }

        public double TempoDias
        {
            get => _tempoDias;
            set
            {
                _tempoDias = value;
                OnPropertyChanged();
            }
        }

        public double? Resultado
        {
            get => _resultado;
            set
            {
                _resultado = value;
                OnPropertyChanged();
            }
        }

        public double? ResultadoDelta
        {
            get => _resultadoDelta;
            set
            {
                _resultadoDelta = value;
                OnPropertyChanged();
            }
        }

        public double? ResultadoGamma
        {
            get => _resultadoGamma;
            set
            {
                _resultadoGamma = value;
                OnPropertyChanged();
            }
        }

        public double? ResultadoTheta
        {
            get => _resultadoTheta;
            set
            {
                _resultadoTheta = value;
                OnPropertyChanged();
            }
        }

        public double? ResultadoVega
        {
            get => _resultadoVega;
            set
            {
                _resultadoVega = value;
                OnPropertyChanged();
            }
        }

        public double? ResultadoRho
        {
            get => _resultadoRho;
            set
            {
                _resultadoRho = value;
                OnPropertyChanged();
            }
        }

        private async void Calcular()
        {
            double T = TempoDias / 365.0; // Convertendo tempo de dias para anos
            double sigma = VolatilidadePorcentagem / 100.0; // Convertendo volatilidade para decimal
            double r = JurosPorcentagem / 100.0; // Convertendo juros para decimal

            double d1 = (Math.Log(PrecoAcao / PrecoStrike) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            double d2 = d1 - sigma * Math.Sqrt(T);

            if(OpcaoSelecionada is null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "Selecione uma Opção", "OK");
                return;
            }

            Resultado = double.Parse(OpcaoSelecionada == 0 ? CalcularCall(d1, d2, r, T).ToString("0.0000") : CalcularPut(d1, d2, r, T).ToString("0.0000"));
            ResultadoDelta = double.Parse(OpcaoSelecionada == 0 ? CallDelta(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000") : PutDelta(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000"));
            ResultadoGamma = double.Parse(OpcaoSelecionada == 0 ? CallGamma(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000") : PutGamma(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000"));
            ResultadoTheta = double.Parse(OpcaoSelecionada == 0 ? CallTheta(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000") : PutTheta(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000"));
            ResultadoVega = double.Parse(OpcaoSelecionada == 0 ? CallVega(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000") : PutVega(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000"));
            ResultadoRho = double.Parse(OpcaoSelecionada == 0 ? CallRho(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000") : PutRho(PrecoAcao, PrecoStrike, T, r, sigma).ToString("0.0000"));
        }

        private void Limpar()
        {
            Resultado = null;
            ResultadoDelta = null;
            ResultadoGamma = null;
            ResultadoTheta = null;
            ResultadoVega = null;
            ResultadoRho = null;
            PrecoAcao = 0;
            PrecoStrike = 0;
            VolatilidadePorcentagem = 0;
            JurosPorcentagem = 0;
            TempoDias = 0;
        }

        private double CalcularPut(double d1, double d2, double r, double t)
        {
            return PrecoStrike * Math.Exp(-r * t) * GaussianCumulativeDistribution(-d2) - PrecoAcao * GaussianCumulativeDistribution(-d1);
        }

        private double CalcularCall(double d1, double d2, double r, double t)
        {
            return PrecoAcao * GaussianCumulativeDistribution(d1) - PrecoStrike * Math.Exp(-r * t) * GaussianCumulativeDistribution(d2);
        }

        private double CallDelta(double S, double X, double T, double r, double sigma)
        {
            double d1 = (Math.Log(S / X) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            return GaussianCumulativeDistribution(d1);
        }

        private double PutDelta(double S, double X, double T, double r, double sigma)
        {
            double d1 = (Math.Log(S / X) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            return GaussianCumulativeDistribution(d1) - 1;
        }

        private double CallGamma(double S, double X, double T, double r, double sigma)
        {
            double d1 = (Math.Log(S / X) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            return GaussianDensityFunction(d1) / (S * sigma * Math.Sqrt(T));
        }
        private double PutGamma(double S, double X, double T, double r, double sigma)
        {
            return CallGamma(S, X, T, r, sigma);
        }

        private double CallTheta(double S, double X, double T, double r, double sigma)
        {
            double d1 = (Math.Log(S / X) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            double d2 = d1 - sigma * Math.Sqrt(T);
            return -((S * GaussianDensityFunction(d1) * sigma) / (2 * Math.Sqrt(T))) - r * X * Math.Exp(-r * T) * GaussianCumulativeDistribution(d2);
        }

        private double PutTheta(double S, double X, double T, double r, double sigma)
        {
            double d1 = (Math.Log(S / X) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            double d2 = d1 - sigma * Math.Sqrt(T);
            return -((S * GaussianDensityFunction(d1) * sigma) / (2 * Math.Sqrt(T))) + r * X * Math.Exp(-r * T) * GaussianCumulativeDistribution(-d2);
        }

        private double CallVega(double S, double X, double T, double r, double sigma)
        {
            double d1 = (Math.Log(S / X) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            return S * Math.Sqrt(T) * GaussianDensityFunction(d1);
        }

        private double PutVega(double S, double X, double T, double r, double sigma)
        {
            return CallVega(S, X, T, r, sigma);
        }

        private double CallRho(double S, double X, double T, double r, double sigma)
        {
            double d1 = (Math.Log(S / X) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            double d2 = d1 - sigma * Math.Sqrt(T);
            return X * T * Math.Exp(-r * T) * GaussianCumulativeDistribution(d2);
        }

        private double PutRho(double S, double X, double T, double r, double sigma)
        {
            double d1 = (Math.Log(S / X) + (r + Math.Pow(sigma, 2) / 2) * T) / (sigma * Math.Sqrt(T));
            double d2 = d1 - sigma * Math.Sqrt(T);
            return -X * T * Math.Exp(-r * T) * GaussianCumulativeDistribution(-d2);
        }

        private static double GaussianCumulativeDistribution(double x)
        {
            double b1 = 0.319381530;
            double b2 = -0.356563782;
            double b3 = 1.781477937;
            double b4 = -1.821255978;
            double b5 = 1.330274429;
            double p = 0.2316419;
            double c2 = 0.39894228;
            double a = Math.Abs(x);
            double t = 1.0 / (1.0 + a * p);
            double b = c2 * Math.Exp((-x) * x / 2.0);
            double n = ((((b5 * t + b4) * t + b3) * t + b2) * t + b1) * t;
            n = 1.0 - b * n;
            if (x < 0.0) n = 1.0 - n;
            return n;
        }

        private static double GaussianDensityFunction(double x)
        {
            return Math.Exp(-Math.Pow(x, 2) / 2) / Math.Sqrt(2 * Math.PI);
        }
    }
}


