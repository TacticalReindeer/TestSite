using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;

public class Library
{
    #region enum
    public enum Direction : byte
    {
        Null, Up, Down, Right, Left, Origin
    }
    #endregion
    public static T[] Reorder<T>(IList<T> array, Func<T, T, bool> rule)
    {
        byte indexOfGreatest;
        for (byte i = 0; i < array.Count - 1; i++)
        {
            indexOfGreatest = i;
            for (byte j = (byte)(i + 1); j < array.Count; j++)
                if (rule(array[indexOfGreatest], array[j])) indexOfGreatest = j;
            T cache = array[i];
            array[i] = array[indexOfGreatest];
            array[indexOfGreatest] = cache;
        }
        return (T[])array;
    }
    #region ConsoleTools
    public abstract class Item
    {
        public virtual string Name { get => _Name; set => _Name = value; }
        protected string _Name = "";
        public virtual Vector Position
        {
            get => _Position; set
            {
                if (value == Position) return;
                Clear();
                _Position = value;
                Print();
            }
        }
        protected Vector _Position;
        public byte Height { get => _Height; }
        protected byte _Height = 1;
        public abstract string FullContent { get; }
        protected string CurrentContent;

        protected Item(string name = "")
        {
            _Name = name;
            _Position = Console.GetCursorPosition();
        }

        public virtual void Print()
        {
            Console.SetCursorPosition(Position.x, Position.y);
            string fullContent = FullContent;
            Console.Write(fullContent);
            _Height = (byte)(Console.CursorTop - Position.y + 1);
            CurrentContent = fullContent;
        }
        public virtual void Clear()
        {
            Console.SetCursorPosition(Position.x, Position.y);
            Console.Write(GetClearStr(CurrentContent));
            CurrentContent = "";
        }
        protected virtual void Update()
        {
            Console.SetCursorPosition(Position.x, Position.y);
            string fullContent = FullContent;
            PreciseWrite(CurrentContent, fullContent);
            CurrentContent = fullContent;
        }

        public override string ToString() => Name;
    }
    public class Text : Item
    {
        public string Content
        {
            get => _Content; set
            {
                if (value == Content) return;
                _Content = value;
                Update();
            }
        }
        private string _Content;
        public Direction AlignDirection
        {
            get => _AlignDirection; set
            {
                if (value == AlignDirection) return;
                _AlignDirection = value;
                Update();
            }
        }
        protected Direction _AlignDirection = Direction.Left;
        public byte AlignOffset
        {
            get => _AlignOffset; set
            {
                if (value == AlignOffset) return;
                _AlignOffset = value;
                Update();
            }
        }
        protected byte _AlignOffset = 0;
        public bool UseAbsolutePosition
        {
            get => _UseAbsolutePosition; set
            {
                if (value == UseAbsolutePosition) return;
                _UseAbsolutePosition = value;
                Update();
            }
        }
        protected bool _UseAbsolutePosition = false;
        public override string FullContent
        {
            get
            {
                string str = "";
                for (byte i = AlignDirection == Direction.Right ? (byte)(Content.Length - 1) : (byte)0; i < AlignOffset; i++) str += " ";
                str += Content;
                return str;
            }
        }

        public override void Clear()
        {
            base.Clear();
            _Content = "";
        }

        public Text(string content, string name = "", Direction alignDirection = Direction.Left, byte alignOffset = 0, bool useAbsolutePosition = false)
            : base(name)
        {
            _Content = content;
            _AlignDirection = alignDirection;
            _AlignOffset = alignOffset;
            _UseAbsolutePosition = useAbsolutePosition;
            Print();
        }
    }
    public class Data : Item
    {
        public string Content { get => _Content.Content; set => _Content.Content = value; }
        protected Text _Content;
        public Direction AlignDirection { get => _Content.AlignDirection; set => _Content.AlignDirection = value; }
        public byte AlignOffset { get => _Content.AlignOffset; set => _Content.AlignOffset = value; }
        public bool UseAbsolutePosition { get => _Content.UseAbsolutePosition; set => _Content.UseAbsolutePosition = value; }
        public override string FullContent
        {
            get
            {
                string str = Name;
                str += ": ";
                str += _Content.FullContent;
                return str;
            }
        }

        public Data(string name = "", string content = "", Direction alignDirection = Direction.Left, byte alignOffset = 0, bool useAbsolutePosition = false)
            : base(name)
        {
            Console.Write(name + ": ");
            _Content = new Text(content, "Content", alignDirection, alignOffset, useAbsolutePosition);
            CurrentContent = FullContent;
        }

        public override void Clear()
        {
            _Content.Clear();
            CurrentContent = FullContent;
            base.Clear();
        }
    }
    public class LoadingBar : Data
    {
        private byte BarLength { get => (byte)(Length * Percent / 100); }
        public byte Percent { get => (byte)((Value * 100) / (MaxValue - MinValue)); }
        public bool ShowData
        {
            get => _ShowData; set
            {
                if (value == ShowData) return;
                _ShowData = value;
                Content = Bar;
            }
        }
        private bool _ShowData = true;
        public string DisplayData { get => _DisplayData; set => _DisplayData = value; }
        private string _DisplayData;
        public byte Value
        {
            get => _Value; set
            {
                if (value == Value) return;
                _Value = value;
                Content = Bar;
            }
        }
        private byte _Value;
        public byte MaxValue
        {
            get => _MaxValue; set
            {
                if (value == MaxValue) return;
                _MaxValue = value;
                Content = Bar;
            }
        }
        private byte _MaxValue = 100;
        public byte MinValue
        {
            get => _MinValue; set
            {
                if (value == MinValue) return;
                _MinValue = value;
                Content = Bar;
            }
        }
        private byte _MinValue = 0;
        public byte Length
        {
            get => _Length; set
            {
                if (value == Length) return;
                _Length = value;
                Content = Bar;
            }
        }
        private byte _Length = 51;
        public string OutputChar
        {
            get => _OutputChar; set
            {
                if (value == OutputChar) return;
                _OutputChar = value.Length switch
                {
                    1 or 3 or 4 => value,
                    _ => "[=>]",
                };
                Content = Bar;
            }
        }
        private string _OutputChar = "[=>]";
        private char LeftEdgeChar { get => OutputChar.Length > 2 ? OutputChar[0] : ' '; }
        private char BarChar { get => OutputChar.Length > 2 ? OutputChar[1] : OutputChar[0]; }
        private char BarEndChar { get => OutputChar.Length > 3 ? OutputChar[2] : ' '; }
        private char RightEdgeChar { get => OutputChar.Length > 2 ? OutputChar[OutputChar.Length - 1] : ' '; }
        private string Bar
        {
            get
            {
                char[] bar = new char[Length];
                byte barLength = BarLength;
                for (byte index = 0; index < bar.Length; index++)
                {
                    if (index < barLength) bar[index] = BarChar;
                    else if (index == barLength && BarEndChar != ' ') bar[index] = BarEndChar;
                    else bar[index] = ' ';
                }
                if (LeftEdgeChar != ' ')
                {
                    bar[0] = LeftEdgeChar;
                    bar[bar.Length - 1] = RightEdgeChar;
                }
                string str = new string(bar);
                if (ShowData)
                {
                    switch (DisplayData)
                    {
                        case "Value":
                            str += " " + Value;
                            break;
                        case "Percentage":
                            str += String.Format("{0,4}%", Percent);
                            break;
                    }
                }
                return str;
            }
        }

        public LoadingBar(string name = "loadingBar", byte value = 0, byte maxValue = 100, byte minValue = 0, byte Length = 50)
            : base(name, "")
        {
            _Value = value;
            _MaxValue = maxValue;
            _MinValue = minValue;
            _Length = Length;
            Content = Bar;
        }
    }
    public class ItemGroup : Item
    {
        public List<Item> Items { get => _Items; }
        private List<Item> _Items;
        public override string FullContent { get; }

        public ItemGroup(string name, List<Item> items)
            : base(name)
        {
            _Items = items;
            _Name = name;
        }

        public override void Print()
        {
            Console.SetCursorPosition(Position.x, Position.y);
            foreach (Item item in Items)
            {
                item.Position = Console.GetCursorPosition();
                item.Print();
                Console.SetCursorPosition(0, item.Position.y + item.Height);
            }
        }
        public override void Clear()
        {
            Items.ForEach(x => x.Clear());
        }
    }
    public static void PreciseWrite(string original, string update)
    {
        string str = "";
        byte position = (byte)Console.CursorLeft;
        byte i;
        for (i = 0; i < original.Length && i < update.Length; i++)
        {
            if (original[i] == update[i])
            {
                if (str != "")
                {
                    Console.Write(str);
                    str = "";
                }
                continue;
            }
            else
            {
                if (str == "")
                {
                    Console.CursorLeft = position + i;
                }
                str += update[i];
            }
        }
        Console.Write(str);
        Console.CursorLeft = position + i;
        if (original.Length > update.Length)
        {
            str = original.Substring(update.Length);
            Console.Write(GetClearStr(str));
        }
        else if (original.Length < update.Length)
        {
            str = update.Substring(original.Length);
            Console.Write(str);
        }
    }
    public static string GetClearStr(string str)
    {
        string output = "";
        foreach (char ch in str)
        {
            switch (ch)
            {
                case '\n':
                    output += '\n';
                    break;
                case '\r':
                    break;
                default:
                    int i = (int)ch;
                    if (i == 12288 || (i > 64312 && i < 65339)) output += "　";
                    else output += ' ';
                    break;
            }
        }
        return output;
    }
    #endregion
    public struct Vector
    {
        public Vector() { }
        public Vector(sbyte x, sbyte y) { this.x = x; this.y = y; }
        public Vector(int x, int y) { this.x = (sbyte)x; this.y = (sbyte)y; }
        public sbyte x, y;
        public static Vector one => (1, 1);
        public static Vector zero => (0, 0);
        public static Vector up => (0, 1);
        public static Vector down => (0, -1);
        public static Vector right => (1, 0);
        public static Vector left => (-1, 0);
        public static implicit operator (sbyte, sbyte)(Vector vector) => (vector.x, vector.y);
        public static implicit operator Vector((sbyte x, sbyte y) vector) => new Vector(vector.x, vector.y);
        public static implicit operator Vector((int x, int y) vector) => new Vector((sbyte)vector.x, (sbyte)vector.y);
        public static Vector operator +(Vector left, Vector right) => (left.x + right.x, left.y + right.y);
        public static Vector operator -(Vector left, Vector right) => (left.x - right.x, left.y - right.y);
        public static Vector operator -(Vector vector) => (-vector.x, -vector.y);
        public static bool operator ==(Vector left, Vector right) => left.x == right.x && left.y == right.y;
        public static bool operator !=(Vector left, Vector right) => !(left == right);

        public override bool Equals(Object obj)
        {
            if (!(obj is Vector)) return false;
            return this == (Vector)obj;
        }
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => $"({x}, {y})";
    }
    public class Piece
    {
        public enum RotationalSymmetryForm : byte
        {
            /// <summary>
            /// 無旋轉對稱
            /// </summary>
            None,

            /// <summary>
            /// 180度旋轉對稱
            /// </summary>
            FlatAngle,

            /// <summary>
            /// 90度旋轉對稱
            /// </summary>
            RigleAngle
        }

        public Piece(Piece piece)
        {
            this.ID = piece.ID;
            this.Position = piece.Position;
            this._Parts = (Vector[])piece.Parts.Clone();
            this._RotationalSymmetry = piece.RotationalSymmetry;
            this._Symmetric = piece.Symmetric;
            this._RotationalAngle = piece.RotationalAngle;
            this._DL_Boundary = piece.DL_Boundary;
            this._UR_Boundary = piece.UR_Boundary;
        }
        public Piece(Vector[] Parts) => this._Parts = Parts;
        public Piece(Vector Position, Vector[] Parts)
        {
            this.Position = Position;
            this._Parts = Parts;
        }
        public Piece(byte ID, Vector Position, Vector[] Parts)
        {
            this.ID = ID;
            this.Position = Position;
            this._Parts = Parts;
        }

        #region Properties
        public readonly byte ID;
        public Vector Position;
        public Vector[] Parts { get => _Parts; }
        private Vector[] _Parts;
        public RotationalSymmetryForm RotationalSymmetry { get => _RotationalSymmetry; }
        private RotationalSymmetryForm _RotationalSymmetry;
        public bool Symmetric { get => _Symmetric; }
        private bool _Symmetric;
        public byte RotationalAngle
        {
            get => _RotationalAngle;
            set => SetRotationalAngle(value);
        }
        private byte _RotationalAngle = 0;
        public Vector DL_Boundary { get => _DL_Boundary; }
        private Vector _DL_Boundary;
        public Vector UR_Boundary { get => _UR_Boundary; }
        private Vector _UR_Boundary;
        public byte SideLength { get => _SideLength; }
        private byte _SideLength = 0;
        #endregion

        #region Methods
        public void RefreshAllProperty()
        {
            ReorderParts();
            CaculateBoundary();
            ResetPosition();
            CaculateSideLength();
            SymmerticCheck();
        }
        public void CaculateBoundary()
        {
            _DL_Boundary = Vector.zero;
            _UR_Boundary = Vector.zero;
            foreach (Vector part in Parts)
            {
                if (part.x > _UR_Boundary.x) _UR_Boundary.x = part.x;
                else if (part.x < _DL_Boundary.x) _DL_Boundary.x = part.x;
                if (part.y > _UR_Boundary.y) _UR_Boundary.y = part.y;
                else if (part.y < _DL_Boundary.y) _DL_Boundary.y = part.y;
            }
        }
        private void SetRotationalAngle(byte rotationalAngle)
        {
            if (rotationalAngle > 7) throw new Exception("RotationalAngle must be between 0~7");

            sbyte from = (sbyte)_RotationalAngle, to = (sbyte)rotationalAngle;
            bool flip = (from > 3) != (to > 3);
            if (flip) to *= -1;
            FlipAndRotate(flip, (sbyte)((to - from) * (flip ? -1 : 1)));

            _RotationalAngle = rotationalAngle;
        }
        public Piece FlipAndRotate(bool flip = false, sbyte numOfSpins = 1)
        {
            // 計算實際上需要旋轉的次數
            numOfSpins %= 4;
            if (numOfSpins < 0) numOfSpins += 4;

            // 旋轉參數
            // 透過 對調XY 反轉X 反轉Y 可以旋轉或翻轉拼圖
            bool swapAxis = false,// 對調XY軸的值
                 xFlip = false,// X值反轉
                 yFlip = false;// Y值反轉

            switch (flip, numOfSpins)
            {
                case (false, 0):// 無須翻轉或旋轉:立即傳回
                    return this;
                case (false, 1):// 順時針旋轉90度:先對調XY再反轉Y即可
                    swapAxis = true;
                    yFlip = true;
                    break;
                case (false, 2):
                    xFlip = true;
                    yFlip = true;
                    break;
                case (false, 3):
                    swapAxis = true;
                    xFlip = true;
                    break;
                case (true, 0):
                    xFlip = true;
                    break;
                case (true, 1):// 先沿y軸翻轉再順時針旋轉90度:對調XY
                    swapAxis = true;
                    break;
                case (true, 2):
                    yFlip = true;
                    break;
                case (true, 3):
                    swapAxis = true;
                    yFlip = true;
                    xFlip = true;
                    break;
            }
            // 旋轉拼圖
            for (byte index = 0; index < this.Parts.Length; index++) this.Parts[index] = Spin(this.Parts[index]);

            // 旋轉邊界座標
            _DL_Boundary = Spin(_DL_Boundary);
            _UR_Boundary = Spin(_UR_Boundary);

            // 調整邊界座標
            Vector i = _UR_Boundary;
            if (xFlip)
            {
                _UR_Boundary.x = _DL_Boundary.x;
                _DL_Boundary.x = i.x;
            }
            if (yFlip)
            {
                _UR_Boundary.y = _DL_Boundary.y;
                _DL_Boundary.y = i.y;
            }
            _UR_Boundary -= _DL_Boundary;
            for (byte index = 0; index < Parts.Length; index++) Parts[index] -= _DL_Boundary;
            _DL_Boundary = (0, 0);

            return this;

            // 單一座標旋轉
            Vector Spin(Vector vector)
            {
                if (swapAxis) vector = new Vector(vector.y, vector.x);
                if (xFlip) vector.x *= -1;
                if (yFlip) vector.y *= -1;
                return vector;
            }
        }
        public void SymmerticCheck()
        {
            // 重置邊界座標
            CaculateBoundary();

            // 宣告實驗拼圖
            Piece test = new Piece(this);

            // 重置對稱屬性
            RotationalSymmetryForm rotationalSymmetry = RotationalSymmetryForm.None;
            bool symmetric = false;

            // 製作對照布林陣列
            bool[,] controlPattern = new bool[UR_Boundary.x + 1, UR_Boundary.y + 1];
            foreach (Vector vector in Parts) controlPattern[vector.x, vector.y] = true;

            // 90度旋轉對稱檢查
            if (CheckOverlap())
            {
                test.FlipAndRotate();
                rotationalSymmetry = RotationalSymmetryForm.RigleAngle;
            }

            // 180度旋轉對稱檢查
            else if (CheckOverlap()) rotationalSymmetry = RotationalSymmetryForm.FlatAngle;

            // y軸對稱檢查
            symmetric |= CheckOverlap(true, 2);

            // x軸對稱檢查
            symmetric |= CheckOverlap(numOfSpins: 2);

            // 45度斜角對稱檢查
            symmetric |= CheckOverlap();

            // 135度斜角對稱檢查
            symmetric |= CheckOverlap(numOfSpins: 2);

            _RotationalSymmetry = rotationalSymmetry;
            _Symmetric = symmetric;

            // 旋轉然後檢查重疊
            bool CheckOverlap(bool flip = false, byte numOfSpins = 1)
            {
                // 旋轉，貼齊左下邊界至原點
                test.FlipAndRotate(flip, (sbyte)numOfSpins);

                // 檢查重疊
                try
                {
                    foreach (Vector vector in test.Parts) if (!controlPattern[vector.x, vector.y]) return false;
                    return true;
                }
                catch (IndexOutOfRangeException) { return false; }
            }
        }
        public void RedefineRotationalAngle(byte rotationalAngle)
        {
            if (rotationalAngle > 7) throw new Exception("RotationalAngle must be between 0~7");
            _RotationalAngle = rotationalAngle;
        }
        public static IEnumerable<(bool[] node, Vector chess, Direction direction)> EdgeFollower(Piece piece)
        {
            Vector chess = piece.Parts[0];
            List<Vector> parts = new List<Vector>(piece.Parts);
            bool[] node;
            Direction direction = Direction.Right;

            // Move chess to the edge of the piece
            while (parts.Exists(x => x == chess)) chess.y++;
            chess.y--;

            Vector origin = chess;

            do
            {
                node = new bool[]
                {
                    parts.Exists(x => x == chess + Vector.up),
                    parts.Exists(x => x == chess + Vector.one),
                    parts.Exists(x => x == chess),
                    parts.Exists(x => x == chess + Vector.right)
                };

                yield return (node, chess, direction);

                switch (direction)
                {
                    case Direction.Down:
                        if (!node[2])// left?
                        {
                            move(Vector.left, Direction.Left);
                            break;
                        }
                        goto case Direction.Right;

                    case Direction.Right:
                        if (!node[3])
                        {
                            move(Vector.down, Direction.Down);
                            break;
                        }
                        goto case Direction.Up;

                    case Direction.Up:
                        if (!node[1])
                        {
                            move(Vector.right, Direction.Right);
                            break;
                        }
                        goto case Direction.Left;

                    case Direction.Left:
                        if (!node[0])
                        {
                            move(Vector.up, Direction.Up);
                            break;
                        }
                        goto case Direction.Down;
                }

                void move(Vector delta, Direction dir)
                {
                    chess += delta;
                    direction = dir;
                }
            } while (chess != origin);

            yield break;
        }
        public string GetPatternStr()
        {
            string patternStr = "";
            bool[,] pattern = GetPatternArray();
            for (byte y = (byte)(pattern.GetLength(1) - 1); y < pattern.GetLength(1); y--)
            {
                for (byte x = 0; x < pattern.GetLength(0); x++) patternStr += pattern[x, y] ? "Ｘ" : "　";
                patternStr += "\n";
            }
            return patternStr;
        }
        public string GetInfoStr()
        {
            string partInfo = "";
            foreach (Vector vector in Parts) partInfo += vector + "\n";

            return
            "\nPiece info:\n" +
            this.GetPatternStr() +
            $"ID: {ID}\n" +
            $"Position: {Position}\n" +
            "Parts: \n" + partInfo +
            $"Symmetric: {Symmetric}\n" +
            $"RotationalSymmetry: {RotationalSymmetry}\n" +
            $"RotationalAngle: {RotationalAngle}\n" +
            $"DL_Boundary: {_DL_Boundary}\n" +
            $"UR_Boundary: {_UR_Boundary}\n" +
            $"SideLength: {SideLength}";
        }
        public void ResetPosition()
        {
            for (byte i = 0; i < Parts.Length; i++) Parts[i] -= DL_Boundary;
            _UR_Boundary -= DL_Boundary;
            _DL_Boundary = (0, 0);
        }
        public int CaculateSideLength()
        {
            _SideLength = 0;
            foreach ((bool[] var, Vector position, bool Xaxis) current in AxisScanner()) if (current.var[0] != current.var[1]) _SideLength++;
            return SideLength;
        }
        public bool[,] GetPatternArray() => GetPatternArray((0, 0), (0, 0));
        public bool[,] GetPatternArray(Vector DL_padding, Vector UR_padding)
        {
            bool[,] pattern = new bool[UR_Boundary.x + 1 + DL_padding.x + UR_padding.x, UR_Boundary.y + 1 + DL_padding.y + UR_padding.y];
            foreach (Vector part in Parts) pattern[part.x + DL_padding.x, part.y + DL_padding.y] = true;
            return pattern;
        }
        public IEnumerable<(bool[] var, Vector position, bool Xaxis)> AxisScanner()
        {
            // Place the puzzle pieces in an array with 1 space around them
            bool[,] pattern = GetPatternArray((1, 1), (1, 1));
            sbyte xBoundary = (sbyte)pattern.GetLength(0), yBoundary = (sbyte)pattern.GetLength(1);

            // x axis scan
            for (sbyte y = 0; y < yBoundary - 1; y++)
                for (sbyte x = 0; x < xBoundary; x++) yield return (new bool[] { pattern[x, y], pattern[x, y + 1] }, (x, y), true);

            // y axis scan
            for (sbyte x = 0; x < xBoundary - 1; x++)
                for (sbyte y = 0; y < yBoundary; y++) yield return (new bool[] { pattern[x, y], pattern[x + 1, y] }, (x, y), false);
        }
        public void ReorderParts() => _Parts = Reorder<Vector>(Parts, (left, right) => right.y > left.y || (right.y == left.y && right.x > left.x));
        #endregion

        public struct PuzzleSolution
        {
            public PuzzleSolution((byte ID, Vector position, byte RotationalAngle)[] solution) { Solution = solution; }

            public (byte ID, Vector position, byte RotationalAngle)[] Solution;

            public static implicit operator PuzzleSolution((byte ID, Vector position, byte RotationalAngle)[] solution) => new PuzzleSolution(solution);
            public static implicit operator (byte ID, Vector position, byte RotationalAngle)[](PuzzleSolution solution) => solution.Solution;
        }
    }
}