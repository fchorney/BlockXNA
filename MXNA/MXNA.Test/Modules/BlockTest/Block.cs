using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using MXNA.Input;
using MXNA.Drawing;

namespace MXNA.Test.Modules
{
    enum BlockType
    {
        Empty = -1,
        Yellow,
        Blue,
        Red,
        Green,
        White
    }

    enum BlockState
    {
        Normal,
        Swapping,
        Exploding,
        Exploded,
        Suspend
    }

    interface IBlockContainer
    {
        float X { get; set; }
        float Y { get; set; }
        float OffsetX { get; }
        float OffsetY { get; }
        Block[,] Blocks { get; }
        int NumRows { get; }
        int NumCols { get; }
    }

    class Block
    {
        private BlockType _Type;
        private BlockState _State;
        private double _TimeInState;

        public Color Color;

        public BlockType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public BlockState State
        {
            get { return _State; }
            set { 
                _State = value;
                _TimeInState = 0;
            }
        }

        public double TimeInState
        {
            get { return _TimeInState; }
        }

        public bool IsEmpty
        {
            get { return _Type == BlockType.Empty; }
        }

        public bool IsSwapping
        {
            get { return _State == BlockState.Swapping; }
        }

        public bool IsExploding
        {
            get { return _State == BlockState.Exploding; }
        }

        public bool IsExploded
        {
            get { return _State == BlockState.Exploded; }
        }

        public Block() { }

        public Block(BlockType type) 
        {
            _Type = type;
            State = BlockState.Normal;

            Color = Color.White;
        }

        public void Update(GameTime gameTime)
        {
            _TimeInState += gameTime.ElapsedGameTime.TotalSeconds;

            if (IsExploding)
            {
                Color.A = (byte)((_TimeInState * 255) % 255);
            }
        }
    }

    class BlockGroup : IBlockContainer 
    {
        float _X;
        float _Y;
        int _Row;
        int _Col;
        int _NumRows;
        int _NumCols;
        BlockGrid _Parent;
        double _Age;

        Block[,] _Blocks;

        public float X
        {
            get { return _X; }
            set
            {
                _X = value;
                _Col = (int)(_X / Parent.CellWidth);
            }
        }

        public float Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                _Row = (int)(_Y / Parent.CellHeight);
            }
        }

        public float OffsetX
        {
            get { return (_Parent == null ? 0 : _Parent.X) + _X; }
        }

        public float OffsetY
        {
            get { return (_Parent == null ? 0 : _Parent.Y) + _Y; }
        }

        public int Row
        {
            get { return _Row; }
            set 
            { 
                _Row = value;
                _Y = _Row * Parent.CellHeight;
            }
        }

        public int Col
        {
            get { return _Col; }
            set {
                _Col = value;
                _X = _Col * Parent.CellWidth;
            }
        }

        public int NumRows
        {
            get { return _NumRows; }
            set { _NumRows = value; }
        }

        public int NumCols
        {
            get { return _NumCols; }
            set { _NumCols = value; }
        }

        public Block[,] Blocks
        {
            get { return _Blocks; }
        }

        public BlockGrid Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        public double Age
        {
            get { return _Age; }
            set { _Age = value; }
        }

        public BlockGroup(int rows, int cols, BlockGrid parent) 
        { 
            _Blocks = new Block[rows,cols];
            _NumRows = rows;
            _NumCols = cols;
            _Parent = parent;
        }

        public void Add(Block block, int row, int col)
        {
            _Blocks[row,col] = block;
        }


    }
}
