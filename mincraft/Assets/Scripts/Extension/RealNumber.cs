using System;

namespace Extension {
    public readonly struct RealNumber:
        IComparable<RealNumber>,
        IComparable,
        IEquatable<RealNumber> 
    {

        private readonly bool _symbol;
        private readonly uint _integer;
        private readonly uint _numerator;
        private readonly uint _denominator;
        private const int UShortLength = 4;
        
        public int RoundInteger => (_symbol ? 1 : -1) * (int)(_integer + ((_numerator << 1) >= _denominator ? 1 : 0));
        private static uint GetInteger(ref uint numerator, uint denominator) {
            var temp = numerator;
            numerator %= denominator;
            return temp / denominator;
        }
        private static RealNumber Minus(RealNumber target) => 
            new(!target._symbol, target._integer, target._numerator, target._denominator);

        #region Operators

        public static RealNumber operator +(RealNumber a, RealNumber b) {
            var gcd = a._denominator.GCD2(b._denominator);
            
            var denominator =  gcd.first * b._denominator;
            var numeratorA = gcd.second * a._numerator;
            var numeratorB = gcd.first * b._numerator;
            
            //same symbol
            if (!a._symbol ^ b._symbol) {

                return new(
                    a._symbol,
                    a._integer + b._integer,
                    numeratorA + numeratorB,
                    denominator
                );
            }

            //integer equal
            if (a._integer == b._integer) {
                if (numeratorA > numeratorB)
                    return new(
                        a._symbol,
                        0,
                        numeratorA - numeratorB,
                        denominator
                    );
                return new(
                    b._symbol,
                    0,
                    numeratorB - numeratorA,
                    denominator
                );
            }
             
            //a > b
            if (a._integer > b._integer) {
                if (numeratorA > numeratorB)
                    return new(
                        a._symbol,
                        a._integer - b._integer,
                        numeratorA - numeratorB,
                        denominator
                    );
                
                return new(
                    a._symbol,
                    a._integer - b._integer - 1,
                    denominator - numeratorB + numeratorA,
                    denominator
                );
            }
            
            //a < b
            if (numeratorA < numeratorB)
                return new(
                    b._symbol,
                     b._integer - a._integer,
                     numeratorB - numeratorA,
                    denominator
                );
                            
            return new(
                b._symbol,
                b._integer - a._integer - 1,
                denominator - numeratorA + numeratorB,
                denominator
            );
        }

        public static RealNumber operator -(RealNumber a, RealNumber b) =>
            a + Minus(b);

        public static RealNumber operator *(RealNumber a, RealNumber b) {

            var gcd1 = a._denominator.GCD2(b._numerator);
            var gcd2 = a._numerator.GCD2(b._denominator);
            var numerator1 = gcd1.second * gcd2.first;
            var denominator1 = gcd1.first * gcd2.second; 

            var gcd3 = a._integer.GCD2(b._denominator);
            var numerator2 = gcd3.first * b._numerator;
            var denominator2 = gcd3.second;
            
            var gcd4 = b._integer.GCD2(a._denominator);
            var numerator3 = gcd4.first * a._numerator;
            var denominator3 = gcd4.second;

            var result = 
                new RealNumber(true, 0, numerator1, denominator1)
                + new RealNumber(true, 0, numerator2, denominator2)
                + new RealNumber(true, 0, numerator3, denominator3);

            return new(
                !a._symbol ^ b._symbol,
                a._integer * b._integer + result._integer,
                result._numerator,
                result._denominator
            );
        }
            
        #endregion
         
        #region Constructor

        public RealNumber(int value) =>
            (_symbol, _integer, _numerator, _denominator) = (value.Symbol(), (uint)(value.Abs()), 0, 1);

        public RealNumber(float value) {

            int iValue = (int)value;
            _symbol = iValue.Symbol();
            _integer = (uint)iValue.Abs();

            value -= iValue;
            
            int tempNumerator = 0;
            int tempDenominator = 1;
            for(int i = 0; i < UShortLength; i++) {
                
                value *= 10;
                if (value < 1) {
                    break;
                }
                tempNumerator = tempNumerator * 10 + (int)value;
                tempDenominator *= 10;
                value -= (int)value;
            }

            (_numerator, _denominator) = 
                ExNumber.GCD2((uint)tempNumerator, (uint)tempDenominator);
        }
        
        public RealNumber(int numerator, int denominator) {

            if (denominator == 0)
                throw new ArgumentException("Denominator can't be 0");
            
            _symbol = !numerator.Symbol() ^ denominator.Symbol();
            (var tempNumerator, var tempDenominator) = 
                ExNumber.GCD2((uint)numerator.Abs(), (uint)denominator.Abs());
            
            _integer = GetInteger(ref tempNumerator, tempDenominator);
            (_numerator, _denominator) = (tempNumerator, tempDenominator);
        }

        public RealNumber(int integer, uint numerator, uint denominator) {
            
            if (denominator == 0)
                throw new ArgumentException("Denominator can't be 0");
            
            _symbol = integer.Symbol();
            
            (numerator, denominator) = numerator.GCD2(denominator);
            _integer = (uint)integer.Abs() + GetInteger(ref numerator, denominator);
            (_numerator, _denominator) = (numerator, denominator);
        }

        private RealNumber(bool symbol, uint integer, uint numerator, uint denominator) {

            if (denominator == 0)
                throw new ArgumentException("Denominator can't be 0");
            
            (numerator, denominator) = numerator.GCD2(denominator);
            integer += GetInteger(ref numerator, denominator);
            (_symbol, _integer, _numerator, _denominator) = (symbol, integer, numerator, denominator);
        }
           
        public static implicit operator RealNumber(int value) => 
            new(value);

        public static implicit operator RealNumber(float value) =>
            new(value);
        
        #endregion

        #region Compare

        public int CompareTo(RealNumber other) {

            if (_symbol != other._symbol) {

                if (this is { _integer: 0, _numerator: 0 } && other is { _integer: 0, _numerator: 0 })
                    return 0;
                
                return _symbol ? 1 : -1;
            }
            
            var result = _integer == other._integer
                ? ((int)_numerator * other._denominator).CompareTo((int)_denominator * other._numerator)
                : _integer.CompareTo(other._integer);

            return result * (_symbol ? 1 : -1);
        }
            
        public int CompareTo(object obj) =>
            obj switch {
                RealNumber r => CompareTo(r),
                int n => ToInt32().CompareTo(n),
                float f => ToDouble().CompareTo(f),
                double d => ToDouble().CompareTo(d),
                decimal m => ToDecimal().CompareTo(m),
                _ => 0
            };
        
        public bool Equals(RealNumber other) =>
            _integer.Equals(other._integer) 
            && _numerator.Equals(other._numerator) 
            && _denominator.Equals(other._denominator);

        #endregion
        
        #region Convert
        public float ToSingle(IFormatProvider provider = null) =>
            Convert.ToSingle(_integer + (float)_numerator / _denominator, provider);
        public decimal ToDecimal(IFormatProvider provider = null) =>
            Convert.ToDecimal(_integer + (decimal)_numerator / _denominator, provider);
        public double ToDouble(IFormatProvider provider = null) =>
            Convert.ToDouble(_integer + (double)_numerator / _denominator, provider);
        public int ToInt32(IFormatProvider provider = null) =>
            RoundInteger;

        public override string ToString() =>
            $"{(_symbol ? "" : "-")}{_integer} {(_symbol ? '+' : '-')} {_numerator} / {_denominator}";

        #endregion
    }
}