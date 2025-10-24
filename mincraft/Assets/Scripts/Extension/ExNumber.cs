namespace Extension {
    public static class ExNumber {
        public static int Abs(this int value) => value >= 0 ? value : -value;
        public static bool Symbol(this int value) => value >= 0;
        public static int GCD(this int value1, int value2) =>
            value1 == 0 ? value2 : GCD(value2 % value1, value1);

        public static (int first, int second) GCD2(this int value1, int value2) {
            var gcd = GCD(value1, value2);
            return (value1 / gcd, value2 / gcd);
        }
        
        public static uint GCD(this uint value1, uint value2) =>
            value1 == 0 ? value2 : GCD(value2 % value1, value1);

        public static (uint first, uint second) GCD2(this uint value1, uint value2) {
            var gcd = GCD(value1, value2);
            return (value1 / gcd, value2 / gcd);
        }
    }
}