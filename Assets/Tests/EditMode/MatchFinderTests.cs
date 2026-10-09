using NUnit.Framework;
using GemBlast.Core;

namespace GemBlast.Tests
{

    [TestFixture]
    public class MatchFinderTests
    {
        [Test]
        public void FindMatches_SingleBlock_ReturnsSingleBlock()
        {
            var board = new Board(3, 3);
            board.SetBlock(1, 1, new BlockData(1, 1, 1, 1));

            var matchFinder = new MatchFinder(board);
            var matches = matchFinder.FindMatches(1, 1);

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(1, matches[0].Id);
        }

        [Test]
        public void FindMatches_TwoHorizontalSameColor_ReturnsBoth()
        {
            var board = new Board(3, 3);
            board.SetBlock(0, 0, new BlockData(1, 0, 0, 1));
            board.SetBlock(1, 0, new BlockData(2, 1, 0, 1));

            var matchFinder = new MatchFinder(board);
            var matches = matchFinder.FindMatches(0, 0);

            Assert.AreEqual(2, matches.Count);
        }

        [Test]
        public void FindMatches_TwoVerticalSameColor_ReturnsBoth()
        {
            var board = new Board(3, 3);
            board.SetBlock(0, 0, new BlockData(1, 0, 0, 1));
            board.SetBlock(0, 1, new BlockData(2, 0, 1, 1));

            var matchFinder = new MatchFinder(board);
            var matches = matchFinder.FindMatches(0, 0);

            Assert.AreEqual(2, matches.Count);
        }

        [Test]
        public void FindMatches_LShapeGroup_ReturnsAll()
        {

            var board = new Board(3, 3);
            board.SetBlock(0, 0, new BlockData(1, 0, 0, 1));
            board.SetBlock(0, 1, new BlockData(2, 0, 1, 1));
            board.SetBlock(1, 0, new BlockData(3, 1, 0, 1));

            var matchFinder = new MatchFinder(board);
            var matches = matchFinder.FindMatches(0, 0);

            Assert.AreEqual(3, matches.Count);
        }

        [Test]
        public void FindMatches_DiagonalNotConnected()
        {

            var board = new Board(3, 3);
            board.SetBlock(0, 0, new BlockData(1, 0, 0, 1));
            board.SetBlock(1, 1, new BlockData(2, 1, 1, 1));

            var matchFinder = new MatchFinder(board);
            var matches = matchFinder.FindMatches(0, 0);

            Assert.AreEqual(1, matches.Count, "Diagonal blocks should not be connected");
        }

        [Test]
        public void FindMatches_DifferentColors_NotConnected()
        {
            var board = new Board(3, 3);
            board.SetBlock(0, 0, new BlockData(1, 0, 0, 1));
            board.SetBlock(1, 0, new BlockData(2, 1, 0, 2));

            var matchFinder = new MatchFinder(board);
            var matches = matchFinder.FindMatches(0, 0);

            Assert.AreEqual(1, matches.Count, "Different colors should not be connected");
        }

        [Test]
        public void FindMatches_LargeConnectedGroup()
        {

            var board = new Board(4, 4);
            int id = 1;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    board.SetBlock(x, y, new BlockData(id++, x, y, 1));
                }
            }

            var matchFinder = new MatchFinder(board);
            var matches = matchFinder.FindMatches(0, 0);

            Assert.AreEqual(16, matches.Count, "All 16 blocks should be connected");
        }

        [Test]
        public void GetMatchCount_ReturnsCorrectCount()
        {
            var board = new Board(3, 3);
            board.SetBlock(0, 0, new BlockData(1, 0, 0, 1));
            board.SetBlock(1, 0, new BlockData(2, 1, 0, 1));
            board.SetBlock(2, 0, new BlockData(3, 2, 0, 1));

            var matchFinder = new MatchFinder(board);
            var block = board.GetBlock(0, 0);
            int count = matchFinder.GetMatchCount(block);

            Assert.AreEqual(3, count);
        }

        [Test]
        public void GetMatchCount_InvalidBlock_ReturnsZero()
        {
            var board = new Board(3, 3);
            var matchFinder = new MatchFinder(board);
            
            var invalidBlock = default(BlockData);
            int count = matchFinder.GetMatchCount(invalidBlock);

            Assert.AreEqual(0, count);
        }

        [Test]
        public void FindMatches_InvalidPosition_ReturnsEmptyList()
        {
            var board = new Board(3, 3);
            var matchFinder = new MatchFinder(board);

            var matches = matchFinder.FindMatches(-1, -1);

            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void FindMatches_EmptyPosition_ReturnsEmptyList()
        {
            var board = new Board(3, 3);

            var matchFinder = new MatchFinder(board);

            var matches = matchFinder.FindMatches(1, 1);

            Assert.AreEqual(0, matches.Count);
        }
    }
}
