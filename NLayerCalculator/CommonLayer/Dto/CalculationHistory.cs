using System;

namespace Calculator.CommonLayer.Dto
{
    public class CalculationHistory
    {
        public double Number1 { get; set; }
        public double Number2 { get; set; }
        public string Operation { get; set; }
        public double Result { get; set; }
        public DateTime Timestamp { get; set; }
        public string Expression { get; set; }  // Yeni eklenen özellik
    }
}
