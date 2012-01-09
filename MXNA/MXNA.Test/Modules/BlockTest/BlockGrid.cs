using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using MXNA.Drawing;

namespace MXNA.Test.Modules
{
    class BlockGrid : ManagedGameComponent, IBlockContainer
    {
        #region Variables

        Sprite blockSprite;
        Sprite selectorSprite;

        ParticleEffect_B burstEffect;
        ParticleEffect_C explodeEffect;

        float _X;
        float _Y;
        int _NumRows;
        int _NumCols;
        Block[,] _Blocks;
        List<BlockGroup> _FreeBlocks;
        List<BlockGroup> _ExplodeBlocks;
        List<BlockGroup> _SuspendBlocks;
        List<BlockGroup[]> _SwapBlocks;

        List<BlockGroup> InactiveFreeGroups;
        List<BlockGroup> InactiveExplodeGroups;
        List<BlockGroup> InactiveSuspendGroups;
        List<BlockGroup[]> InactiveSwapGroups;
        List<int[]> ExplodeQueue;

        int _SelectorRow;
        int _SelectorCol;
        int _CellWidth;
        int _CellHeight;

        public float dropSpeed = 300;
        public float swapSpeed = 500;
        public double explodeTime = 0.5;

        #endregion

        #region Properties

        public float X
        {
            get { return _X; }
            set { _X = value; }
        }

        public float Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        public float OffsetX
        {
            get { return _X; }
        }

        public float OffsetY
        {
            get { return _Y; }
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

        public int CellWidth
        {
            get { return _CellWidth; }
        }

        public int CellHeight
        {
            get { return _CellHeight; }
        }

        public Block[,] Blocks
        {
            get { return _Blocks; }
        }

        public int SelectorRow
        {
            get { return _SelectorRow; }
            set 
            { 
                _SelectorRow = value;
                if (_SelectorRow < 0) _SelectorRow = 0;
                if (_SelectorRow > NumRows - 1) _SelectorRow = NumRows - 1;
            }
        }

        public int SelectorCol
        {
            get { return _SelectorCol; }
            set 
            { 
                _SelectorCol = value;
                if (_SelectorCol < 0) _SelectorCol = 0;
                if (_SelectorCol > NumCols - 2) _SelectorCol = NumCols - 2;
            }
        }

        public ParticleEffect_B BurstEffect
        {
            set { burstEffect = value; }
        }

        public ParticleEffect_C ExplodeEffect
        {
            set { explodeEffect = value; }
        }

        #endregion

        #region Constructors

        public BlockGrid(int cellWidth, int cellHeight, int rows, int cols)
        {
            _FreeBlocks = new List<BlockGroup>();
            _ExplodeBlocks = new List<BlockGroup>();
            _SuspendBlocks = new List<BlockGroup>();
            _SwapBlocks = new List<BlockGroup[]>();

            InactiveFreeGroups = new List<BlockGroup>();
            InactiveExplodeGroups = new List<BlockGroup>();
            InactiveSuspendGroups = new List<BlockGroup>();
            InactiveSwapGroups = new List<BlockGroup[]>();

            ExplodeQueue = new List<int[]>();

            _Blocks = new Block[rows, cols];

            _NumRows = rows;
            _NumCols = cols;
            _CellWidth = cellWidth;
            _CellHeight = cellHeight;

            _SelectorRow = rows / 2 - 1;
            _SelectorCol = cols / 2 - 1;

            Animation frame = AnimationFactory.GenerateAnimation(@"Block", 48, 48, 5, 0);
            blockSprite = new Sprite(frame, new Vector2(200, 200));

            frame = AnimationFactory.GenerateAnimation(@"Selector", 104, 56, 1, 0);
            selectorSprite = new Sprite(frame, new Vector2(200, 200));
        }

        #endregion

        #region Helper Functions

        #region Public

        public void Drop(BlockGroup blockGroup, int col)
        {
            blockGroup.Parent = this;
            blockGroup.Col = col;
            blockGroup.Row = -blockGroup.NumRows;
            _FreeBlocks.Add(blockGroup);
        }

        public void DoSwap()
        {
            BlockGroup[] SwapGroups = { new BlockGroup(1, 1, this), new BlockGroup(1, 1, this) };

            if (!ValidSwap())
            {
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                if (_Blocks[_SelectorRow, _SelectorCol + i] != null)
                {
                    _Blocks[_SelectorRow, _SelectorCol + i].State = BlockState.Swapping;
                    SwapGroups[i].Add(_Blocks[_SelectorRow, _SelectorCol + i], 0, 0);
                }
                else
                {
                    Block emptyBlock = new Block(BlockType.Empty);
                    emptyBlock.State = BlockState.Swapping;

                    SwapGroups[i].Add(emptyBlock, 0, 0);
                    _Blocks[_SelectorRow, _SelectorCol + i] = emptyBlock;
                }

                SwapGroups[i].Row = _SelectorRow;
                SwapGroups[i].Col = _SelectorCol + i;
            }

            _SwapBlocks.Add(SwapGroups);
        }

        public void ExplodeBlock(int row, int col)
        {
            if (_Blocks[row, col] != null && _Blocks[row, col].State == BlockState.Normal)
            {
                BlockGroup bg = new BlockGroup(NumRows, NumCols, this);
                bg.Add(_Blocks[row, col], row, col);
                bg.Blocks[row, col].State = BlockState.Exploding;

                _ExplodeBlocks.Add(bg);
            }
        }

        #endregion

        #region Private

        private bool TryChainExplode(Block b, int r, int c, BlockGroup chainGroup)
        {
            if (InBounds(r, c, this) && !BlockIsEmpty(r, c))
            {
                Block target = _Blocks[r, c];
                if (target.State == BlockState.Normal && target.Type == b.Type && !target.IsEmpty)
                {
                    target.State = BlockState.Exploding;
                    chainGroup.Add(target, r, c);
                    return true;
                }
            }
            return false;
        }

        private bool ValidSwap()
        {
            if (BlockIsEmpty(_SelectorRow, _SelectorCol) && BlockIsEmpty(_SelectorRow, _SelectorCol + 1))
            {
                return false;
            }

            for (int i = 0; i < 2; i++)
            {
                int row = _SelectorRow;
                int col = _SelectorCol + i;
                //can only swap blocks in normal state
                if (_Blocks[row, col] != null && _Blocks[row, col].State != BlockState.Normal)
                    return false;

                //check for free blocks blocking swap
                if (BlockIsEmpty(row, col) && !BlockIsAvailable(row, col))
                        return false;
            }
            
            return true;
        }

        private bool BlockIsAvailable(int r, int c)
        {
            foreach (BlockGroup bg in _FreeBlocks)
            {
                int row = r - bg.Row - 1;
                int col = c - bg.Col;
                if (InBounds(row, col, bg) && bg.Blocks[row, col] != null)
                    return false;
            }
            return true;
        }

        private bool BlockIsEmpty(int r, int c)
        {
            return _Blocks[r, c] == null || (_Blocks[r, c].IsEmpty && !_Blocks[r, c].IsSwapping);
        }

        private void CompressColumn(int col)
        {
            BlockGroup freeGroup = null;
            bool foundEmptySpace = false;

            for (int i = NumRows - 1; i >= 0; i--)
            {
                //empty block means potential for free group to fall
                if (BlockIsEmpty(i,col))
                {
                    if (freeGroup != null)
                        _FreeBlocks.Add(freeGroup);

                    foundEmptySpace = true;
                    freeGroup = null;
                }
                else if (foundEmptySpace && _Blocks[i, col].State == BlockState.Normal)
                {
                    if (freeGroup == null)
                    {
                        //create a new free group to add blocks above the last empty space until the next empty space
                        //currently creates a group spanning the entire column - we can optimize this a little at the expense of readability
                        freeGroup = new BlockGroup(NumRows, 1, this);
                        freeGroup.Col = col;
                    }
                    freeGroup.Add(_Blocks[i, col], i, 0);
                    _Blocks[i, col] = null;
                }
            }

            if (freeGroup != null)
                _FreeBlocks.Add(freeGroup);
        }

        private bool CheckCollision(BlockGroup bg)
        {
            bool Collision = false;

            //check collisions against bottom of grid
            if (bg.Row + bg.NumRows >= this.NumRows)
            {
                for (int i = 0; i < bg.NumCols; i++)
                {
                    if (bg.Blocks[bg.NumRows - (bg.Row + bg.NumRows - this.NumRows) - 1, i] != null)
                        return true;
                }
            }

            //check collisions against other blocks
            for (int r = 0; r < bg.NumRows; r++)
            for (int c = 0; c < bg.NumCols; c++)
                if (bg.Blocks[r, c] != null && 
                   (!InBounds(r + bg.Row + 1, c + bg.Col, this) && (r + bg.Row > 0) || 
                   (InBounds(r + bg.Row, c + bg.Col, this) && _Blocks[r + bg.Row + 1, c + bg.Col] != null)))
                {
                    return true;
                }

            return Collision;
        }

        private void CopyGroup(BlockGroup bg)
        {
            for (int r = 0; r < bg.NumRows; r++)
                for (int c = 0; c < bg.NumCols; c++)
                    if (InBounds(bg.Row + r, bg.Col + c, this) && bg.Blocks[r, c] != null)
                    {
                        if (_Blocks[bg.Row + r, bg.Col +c] == null || _Blocks[bg.Row + r, bg.Col +c].State != BlockState.Exploding)
                            _Blocks[bg.Row + r, bg.Col + c] = bg.Blocks[r, c];
                    }
        }

        private bool InBounds(int r, int c, IBlockContainer container)
        {
            return (r >= 0 && r < container.NumRows && c >= 0 && c < container.NumCols);
        }

        #endregion

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {

            for (int r = 0; r < NumRows; r++)
            for (int c = 0; c < NumCols; c++)
            {
                if (_Blocks[r, c] != null)
                {
                    _Blocks[r, c].Update(gameTime);

                    //clean up empty blocks (we can remove this eventually by ignoring empty blocks?)
                    if (BlockIsEmpty(r, c)) _Blocks[r, c] = null;
                }
            }

            #region Process Free Blocks

            foreach (BlockGroup bg in _FreeBlocks)
            {
                //process block falling
                bg.Y += dropSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //check for collisions
                if (CheckCollision(bg))
                {
                    CopyGroup(bg);
                    InactiveFreeGroups.Add(bg);
                }
            }

            foreach (BlockGroup bg in InactiveFreeGroups)
            {
                _FreeBlocks.Remove(bg);
            }
            InactiveFreeGroups.Clear();

            #endregion

            #region Process Swapping Blocks

            foreach (BlockGroup[] swapGroup in _SwapBlocks)
            {
                int PrevColumn = swapGroup[0].Col;

                swapGroup[0].X += swapSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                swapGroup[1].X -= swapSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (PrevColumn != swapGroup[0].Col)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        swapGroup[i].Col = PrevColumn + ((1 + i) % 2);
                        CopyGroup(swapGroup[i]);
                        swapGroup[i].Blocks[0, 0].State = BlockState.Normal;
                        CompressColumn(swapGroup[i].Col);
                    }

                    InactiveSwapGroups.Add(swapGroup);
                }
            }

            foreach (BlockGroup[] bg in InactiveSwapGroups)
            {
                _SwapBlocks.Remove(bg);
            }
            InactiveSwapGroups.Clear();

            #endregion

            #region Process Exploding Blocks

            foreach (BlockGroup bg in _ExplodeBlocks)
            {
                bool IsChainFinished = true;

                for (int r = 0; r < NumRows; r++)
                for (int c = 0; c < NumCols; c++)
                {
                    Block block = bg.Blocks[r, c];
                    if (block != null && block.IsExploding)
                    {
                        IsChainFinished = false;
                        if (block.TimeInState > explodeTime)
                        {
                            //explode adjacent matching blocks
                            TryChainExplode(block, r - 1, c, bg);
                            TryChainExplode(block, r + 1, c, bg);
                            TryChainExplode(block, r, c - 1, bg);
                            TryChainExplode(block, r, c + 1, bg);

                            //do stuff when finished exploding here
                            block.State = BlockState.Exploded;
                            //CompressColumn(bg.Col);
                        }
                    }
                }

                if (IsChainFinished)
                {
                    InactiveExplodeGroups.Add(bg);
                }
            }

            foreach (BlockGroup bg in InactiveExplodeGroups)
            {
                for (int c = 0; c < NumCols; c++)
                {
                    for (int r = 0; r < NumRows; r++)
                    {
                        Block block = bg.Blocks[r, c];
                        if (block != null)
                        {
                            Color burstColor = Color.White;
                            switch (block.Type)
                            {
                                case BlockType.Blue:
                                    burstColor = new Color(32, 64, 214);
                                    break;
                                case BlockType.Green:
                                    burstColor = new Color(12, 216, 0);
                                    break;
                                case BlockType.Yellow:
                                    burstColor = new Color(255, 255, 35);
                                    break;
                                case BlockType.Red:
                                    burstColor = new Color(248, 0, 0);
                                    break;
                                case BlockType.White:
                                    burstColor = Color.White; 
                                    break;
                            }

                            burstEffect.SourceX = c * CellWidth + OffsetX;
                            burstEffect.SourceY = r * CellHeight + OffsetY;
                            burstEffect.Color = burstColor;
                            burstEffect.Burst();

                            explodeEffect.SourceX = c * CellWidth + OffsetX - 40;
                            explodeEffect.SourceY = r * CellHeight + OffsetY - 40;
                            explodeEffect.Color = burstColor;
                            explodeEffect.Burst();

                            //this.Blocks[r, c] = null;
                            this.Blocks[r, c].State = BlockState.Suspend;
                        }
                    }
                }
                _ExplodeBlocks.Remove(bg);
                _SuspendBlocks.Add(bg);
            }
            InactiveExplodeGroups.Clear();

            #endregion

            #region Process Suspended Blocks

            foreach (BlockGroup bg in _SuspendBlocks)
            {
                bg.Age += gameTime.ElapsedGameTime.TotalSeconds;
                if (bg.Age > explodeTime)
                {
                    InactiveSuspendGroups.Add(bg);
                }
            }

            foreach (BlockGroup bg in InactiveSuspendGroups)
            {
                for (int c = 0; c < NumCols; c++)
                {
                    for (int r = 0; r < NumRows; r++)
                    {
                        Block block = bg.Blocks[r, c];
                        if (block != null)
                        {
                            this.Blocks[r, c] = null;
                        }
                    }
                    CompressColumn(c);
                }
                _SuspendBlocks.Remove(bg);
            }
            InactiveSuspendGroups.Clear();



            #endregion

        }

        #endregion

        #region Drawing

        public override void Draw(GameTime gameTime)
        {
            G.SpriteBatch.Begin();

            for (int r = 0; r < NumRows; r++)
            for (int c = 0; c < NumCols; c++)
            {
                blockSprite.Animation.CurrentFrame = 4;
                blockSprite.X = _X + c * _CellWidth;
                blockSprite.Y = _Y + r * _CellHeight;
                blockSprite.Draw(new Color(255,255,255,50));

                DrawBlock(r,c,this);
            }

            foreach (BlockGroup bg in _FreeBlocks)
            {
                for (int r = 0; r < bg.NumRows; r++)
                for (int c = 0; c < bg.NumCols; c++)
                {
                    //blockSprite.Animation.CurrentFrame = 4;
                    //blockSprite.X = bg.OffsetX + c * _CellWidth;
                    //blockSprite.Y = bg.OffsetY + r * _CellHeight;
                    //blockSprite.Draw(spriteBatch, new Color(Color.Red, 100));

                    DrawBlock(r, c, bg);
                }
            }

            foreach (BlockGroup[] swapGroup in _SwapBlocks)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int r = 0; r < swapGroup[i].NumRows; r++)
                        for (int c = 0; c < swapGroup[i].NumCols; c++)
                        {
                            DrawBlock(r, c, swapGroup[i]);
                        }
                }
            }

            selectorSprite.X = OffsetX + _SelectorCol * _CellWidth - 4;
            selectorSprite.Y = OffsetY + _SelectorRow * _CellHeight - 4;
            selectorSprite.Draw();

            G.SpriteBatch.End();

        }

        private void DrawBlock(int row, int col, IBlockContainer group)
        {
            Block block = group.Blocks[row, col];

            //we only draw swapping blocks inside blockgroups
            if (block != null && block.IsSwapping && group == this)
            {
                return;
            }

            if (block != null && !block.IsEmpty && !(block.State == BlockState.Suspend))
            {
                blockSprite.X = group.OffsetX + col * _CellWidth;
                blockSprite.Y = group.OffsetY + row * _CellHeight;

                if (block.IsExploding)
                {
                    blockSprite.Animation.CurrentFrame = 4;
                    blockSprite.Draw();
                }

                switch (block.Type)
                {
                    case BlockType.Yellow:
                        blockSprite.Animation.CurrentFrame = 0;
                        break;
                    case BlockType.Red:
                        blockSprite.Animation.CurrentFrame = 1;
                        break;
                    case BlockType.Blue:
                        blockSprite.Animation.CurrentFrame = 2;
                        break;
                    case BlockType.Green:
                        blockSprite.Animation.CurrentFrame = 3;
                        break;
                    case BlockType.White:
                        blockSprite.Animation.CurrentFrame = 4;
                        break;
                }

                blockSprite.Draw(block.Color);
            }
        }

        #endregion

    }
}
