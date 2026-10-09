using System.Collections.Generic;

namespace GemBlast.Core
{
    public class MatchFinder
    {
        private Board _board;
        
        private bool[] _visited;
        private Queue<BlockData> _queue;
        
        private int[] _parent;
        private int[] _rank;
        private int[] _groupSize;
        
        private List<BlockData> _countBuffer;
        
        private static readonly int[] dx = { 0, 0, 1, -1 };
        private static readonly int[] dy = { 1, -1, 0, 0 };

        public MatchFinder(Board board)
        {
            _board = board;
            int size = _board.Width * _board.Height;
            _visited = new bool[size];
            _queue = new Queue<BlockData>(size);
            _parent = new int[size];
            _rank = new int[size];
            _groupSize = new int[size];
            _countBuffer = new List<BlockData>(size);
        }

        public void ComputeAllGroups()
        {
            int size = _board.Width * _board.Height;
            
            for (int i = 0; i < size; i++)
            {
                _parent[i] = i;
                _rank[i] = 0;
                _groupSize[i] = 1;
            }
            
            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    BlockData current = _board.GetBlock(x, y);
                    if (!current.IsValid) continue;
                    
                    int currentIdx = GetIndex(x, y);
                    
                    if (x + 1 < _board.Width)
                    {
                        BlockData right = _board.GetBlock(x + 1, y);
                        if (right.IsValid && right.ColorIndex == current.ColorIndex)
                        {
                            Union(currentIdx, GetIndex(x + 1, y));
                        }
                    }
                    
                    if (y + 1 < _board.Height)
                    {
                        BlockData up = _board.GetBlock(x, y + 1);
                        if (up.IsValid && up.ColorIndex == current.ColorIndex)
                        {
                            Union(currentIdx, GetIndex(x, y + 1));
                        }
                    }
                }
            }
        }

        public int GetGroupSize(int x, int y)
        {
            if (!_board.IsValidCoordinate(x, y)) return 0;
            
            BlockData block = _board.GetBlock(x, y);
            if (!block.IsValid) return 0;
            
            int root = Find(GetIndex(x, y));
            return _groupSize[root];
        }

        private int Find(int x)
        {
            if (_parent[x] != x)
            {
                _parent[x] = Find(_parent[x]);
            }
            return _parent[x];
        }

        private void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);
            
            if (rootX == rootY) return;
            
            if (_rank[rootX] < _rank[rootY])
            {
                _parent[rootX] = rootY;
                _groupSize[rootY] += _groupSize[rootX];
            }
            else if (_rank[rootX] > _rank[rootY])
            {
                _parent[rootY] = rootX;
                _groupSize[rootX] += _groupSize[rootY];
            }
            else
            {
                _parent[rootY] = rootX;
                _groupSize[rootX] += _groupSize[rootY];
                _rank[rootX]++;
            }
        }

        public List<BlockData> FindMatches(int startX, int startY)
        {
            List<BlockData> result = new List<BlockData>();
            RunBFS(startX, startY, result);
            return result;
        }

        public List<BlockData> FindMatches(BlockData block)
        {
            if (!block.IsValid) return new List<BlockData>();
            return FindMatches(block.X, block.Y);
        }

        public void FindMatchesNonAlloc(int startX, int startY, List<BlockData> output)
        {
            output.Clear();
            RunBFS(startX, startY, output);
        }

        public int GetMatchCount(BlockData block)
        {
            if (!block.IsValid) return 0;
            _countBuffer.Clear();
            RunBFS(block.X, block.Y, _countBuffer);
            return _countBuffer.Count;
        }

        private void RunBFS(int startX, int startY, List<BlockData> output)
        {
            BlockData startBlock = _board.GetBlock(startX, startY);
            if (!startBlock.IsValid) return;

            int targetColor = startBlock.ColorIndex;
            
            System.Array.Clear(_visited, 0, _visited.Length);
            _queue.Clear();

            _queue.Enqueue(startBlock);
            _visited[GetIndex(startX, startY)] = true;

            while (_queue.Count > 0)
            {
                BlockData current = _queue.Dequeue();
                output.Add(current);

                for (int i = 0; i < 4; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];

                    if (_board.IsValidCoordinate(nx, ny))
                    {
                        int nIndex = GetIndex(nx, ny);
                        if (!_visited[nIndex])
                        {
                            BlockData neighbor = _board.GetBlock(nx, ny);
                            if (neighbor.IsValid && neighbor.ColorIndex == targetColor)
                            {
                                _visited[nIndex] = true;
                                _queue.Enqueue(neighbor);
                            }
                        }
                    }
                }
            }
        }

        private int GetIndex(int x, int y)
        {
            return y * _board.Width + x;
        }
    }
}
