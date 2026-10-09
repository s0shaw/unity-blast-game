using System;
using System.Collections.Generic;

namespace GemBlast.Core
{
    public class BoardMechanics
    {
        private Board _board;
        private MatchFinder _matchFinder;
        private readonly System.Random _rng;
        private int _nextBlockId = 1;
        
        private List<BlockData> _shuffleBuffer;
        private List<BlockData> _forceMatchBuffer;
        private Dictionary<int, List<BlockData>> _colorGroupsCache;
        private List<int> _colorKeysBuffer;

        public BoardMechanics(Board board, int? seed = null)
        {
            _rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
            _board = board;
            _matchFinder = new MatchFinder(board);
            int size = board.Width * board.Height;
            _shuffleBuffer = new List<BlockData>(size);
            _forceMatchBuffer = new List<BlockData>(16);
            _colorGroupsCache = new Dictionary<int, List<BlockData>>(8);
            _colorKeysBuffer = new List<int>(8);
        }

        public void SetNextBlockId(int nextId)
        {
            _nextBlockId = nextId;
        }

        public void Blast(List<BlockData> blocks)
        {
            if (blocks == null) return;

            foreach (var block in blocks)
            {
                if (block.IsValid)
                {
                    var boardBlock = _board.GetBlock(block.X, block.Y);
                    if (boardBlock.Equals(block))
                    {
                        _board.SetBlock(block.X, block.Y, default);
                    }
                }
            }
        }

        public void ApplyGravity()
        {
            for (int x = 0; x < _board.Width; x++)
            {
                int writeY = 0;
                for (int readY = 0; readY < _board.Height; readY++)
                {
                    BlockData block = _board.GetBlock(x, readY);
                    if (block.IsValid)
                    {
                        if (writeY != readY)
                        {
                            _board.SetBlock(x, readY, default);
                            BlockData movedBlock = block.WithPosition(x, writeY);
                            _board.SetBlock(x, writeY, movedBlock);
                        }
                        else
                        {
                            if (block.X != x || block.Y != writeY)
                            {
                                 BlockData corrected = block.WithPosition(x, writeY);
                                 _board.SetBlock(x, writeY, corrected);
                            }
                        }
                        writeY++;
                    }
                }
            }
        }
        
        public void RefillBoard(int colorCount, int seed)
        {
            System.Random rnd = new System.Random(seed);
            
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    if (!_board.GetBlock(x, y).IsValid)
                    {
                        int color = rnd.Next(1, colorCount + 1);
                        BlockData newBlock = new BlockData(_nextBlockId++, x, y, color);
                        _board.SetBlock(x, y, newBlock);
                    }
                }
            }
        }

        public bool IsDeadlocked(int minGroupSize = 2)
        {
            _matchFinder.ComputeAllGroups();
            for (int y = 0; y < _board.Height; y++)
                for (int x = 0; x < _board.Width; x++)
                    if (_matchFinder.GetGroupSize(x, y) >= minGroupSize)
                        return false;
            return true;
        }

        public void ShuffleBoard(int minGroupSize = 2)
        {
            _shuffleBuffer.Clear();
            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    var b = _board.GetBlock(x, y);
                    if (b.IsValid) _shuffleBuffer.Add(b);
                }
            }

            if (_shuffleBuffer.Count < minGroupSize) return;

            _colorKeysBuffer.Clear();
            foreach (var kvp in _colorGroupsCache)
            {
                kvp.Value.Clear();
                _colorKeysBuffer.Add(kvp.Key);
            }
            
            foreach (var b in _shuffleBuffer)
            {
                if (!_colorGroupsCache.TryGetValue(b.ColorIndex, out var list))
                {
                    list = new List<BlockData>(16);
                    _colorGroupsCache[b.ColorIndex] = list;
                }
                list.Add(b);
            }

            int targetColor = -1;
            foreach (var kvp in _colorGroupsCache)
            {
                if (kvp.Value.Count >= minGroupSize)
                {
                    targetColor = kvp.Key;
                    break;
                }
            }

            if (targetColor == -1) 
            {
                 if (_shuffleBuffer.Count >= minGroupSize)
                 {
                     targetColor = _shuffleBuffer[0].ColorIndex;
                     for(int i = 1; i < minGroupSize; i++)
                     {
                         var old = _shuffleBuffer[i];
                         _shuffleBuffer[i] = new BlockData(old.Id, old.X, old.Y, targetColor, old.Icon);
                     }
                 }
                 else
                 {
                     return;
                 }
            }

            ShuffleList(_shuffleBuffer);

            _forceMatchBuffer.Clear();
            int foundCount = 0;
            
            for (int i = _shuffleBuffer.Count - 1; i >= 0; i--)
            {
                if (_shuffleBuffer[i].ColorIndex == targetColor)
                {
                    _forceMatchBuffer.Add(_shuffleBuffer[i]);
                    _shuffleBuffer.RemoveAt(i);
                    foundCount++;
                    if (foundCount == minGroupSize) break;
                }
            }
            
            for (int k = 0; k < _forceMatchBuffer.Count; k++)
            {
                _shuffleBuffer.Insert(k, _forceMatchBuffer[k]);
            }
            
            int listIdx = 0;
            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    if (listIdx < _shuffleBuffer.Count)
                    {
                        BlockData b = _shuffleBuffer[listIdx].WithPosition(x, y);
                        _board.SetBlock(x, y, b);
                        listIdx++;
                    }
                    else
                    {
                        _board.SetBlock(x, y, default);
                    }
                }
            }
        }

        private void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = _rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
