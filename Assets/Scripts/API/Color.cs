using UnityEngine;

namespace API {
    public class Color {
        public static readonly Color Red         = new Color(255, 0,   0);
        public static readonly Color Green       = new Color(0,   128, 0);
        public static readonly Color Blue        = new Color(0,   0,   255);
        public static readonly Color White       = new Color(255, 255, 255);
        public static readonly Color Black       = new Color(0,   0,   0);
        public static readonly Color Yellow      = new Color(255, 255, 0);
        public static readonly Color Cyan        = new Color(0,   255, 255);
        public static readonly Color Magenta     = new Color(255, 0,   255);
        public static readonly Color Brown       = new Color(128, 64,  0);
        public static readonly Color Orange      = new Color(255, 128, 0);
        public static readonly Color Pink        = new Color(255, 192, 203);
        public static readonly Color Purple      = new Color(128, 0,   128);
        public static readonly Color Aqua        = new Color(0,   255, 255);
        public static readonly Color Lime        = new Color(0,   255, 0);
        public static readonly Color Gold        = new Color(255, 215, 0);
        public static readonly Color Gray        = new Color(128, 128, 128);
        public static readonly Color Violet      = new Color(128, 0,   128);
        public static readonly Color DeepPink    = new Color(255, 20,  147);
        public static readonly Color LightGray   = new Color(211, 211, 211);
        public static readonly Color DarkGray    = new Color(169, 169, 169);
        public static readonly Color LightBlue   = new Color(173, 216, 230);
        public static readonly Color LightGreen  = new Color(144, 238, 144);
        public static readonly Color LightCyan   = new Color(224, 255, 255);
        public static readonly Color LightMagenta= new Color(255, 0,   255);
        public static readonly Color LightYellow = new Color(255, 255, 224);
        public static readonly Color LightBrown  = new Color(255, 165, 0);
        public static readonly Color LightOrange = new Color(255, 192, 128);
        public static readonly Color LightPink   = new Color(255, 182, 193);
        public static readonly Color Transparent = new Color(0,   0,   0);
        public static readonly Color Empty       = new Color(-1,  -1,  -1);

        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }
        
        public static bool operator ==(Color a, Color b) => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        public static bool operator !=(Color a, Color b) => !(a == b);
        public override bool Equals(object obj) { if (obj is Color color) { return this == color; } return false; }
        protected bool Equals(Color other) => R == other.R && G == other.G && B == other.B && A == other.A;
        
        public override int GetHashCode() {
            unchecked {
                int hashCode = R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ A.GetHashCode();
                return hashCode;
            }
        }
        
        public Color(string name) {
            switch (name.ToLower().Replace(" ", "")) {
                default:            R = 0;   G = 0;   B = 0;   A = 0;   break;
                case "red":         R = 255; G = 0;   B = 0;   A = 255; break;
                case "green":       R = 0;   G = 128; B = 0;   A = 255; break;
                case "blue":        R = 0;   G = 0;   B = 255; A = 255; break;
                case "white":       R = 255; G = 255; B = 255; A = 255; break;
                case "black":       R = 0;   G = 0;   B = 0;   A = 255; break;
                case "yellow":      R = 255; G = 255; B = 0;   A = 255; break;
                case "cyan":        R = 0;   G = 255; B = 255; A = 255; break;
                case "aqua":        R = 0;   G = 255; B = 255; A = 255; break;
                case "magenta":     R = 255; G = 0;   B = 255; A = 255; break;
                case "brown":       R = 128; G = 64;  B = 0;   A = 255; break;
                case "orange":      R = 255; G = 128; B = 0;   A = 255; break;
                case "purple":      R = 128; G = 0;   B = 128; A = 255; break;
                case "pink":        R = 255; G = 0;   B = 255; A = 255; break;
                case "lime":        R = 0;   G = 255; B = 0;   A = 255; break;
                case "gold":        R = 255; G = 215; B = 0;   A = 255; break;
                case "grey":        R = 128; G = 128; B = 128; A = 255; break;
                case "violet":      R = 128; G = 0;   B = 255; A = 255; break;
                case "turquoise":   R = 0;   G = 255; B = 255; A = 255; break;
                case "deeppink":    R = 255; G = 20;  B = 147; A = 255; break;
                case "all255":      R = 255; G = 255; B = 255; A = 255; break;
                case "transparent": R = 0;   G = 0;   B = 0;   A = 0;   break;
            }
        }

        public static Color FromName(string name) => new Color(name);
        public Color(byte r, byte g, byte b) { R = r; G = g; B = b; A = 255; }
        public Color(byte r, byte g, byte b, byte a) { R = r; G = g; B = b; A = a; }
        public Color(int r, int g, int b) { R = r; G = g; B = b; A = 255; }
        public Color(int r, int g, int b, int a) { R = r; G = g; B = b; A = a; }
        public Color(Color color) { R = color.R; G = color.G; B = color.B; A = color.A; }
        public Color(Color color, byte a) { R = color.R; G = color.G; B = color.B; A = a; }
        public Color(Color color, int a) { R = color.R; G = color.G; B = color.B; A = a; }
        public Color(UnityEngine.Color color) { R = (int)color.r; G = (int)color.g; B = (int)color.b; A = (int)color.a; }

    }
}