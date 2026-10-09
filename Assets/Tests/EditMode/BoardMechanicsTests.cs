using NUnit.Framework;
using GemBlast.Core;

namespace GemBlast.Tests
{

    [TestFixture]
    public class BoardMechanicsDeadlockTests
    {
        #region IsDeadlocked Tests

        [Test]
        public void IsDeadlocked_AllDifferentColors_ReturnsTrue()
        {

            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);

            int id = 1;
            board.SetBlock(0, 0, new BlockData(id++, 0, 0, 1));
            board.SetBlock(1, 0, new BlockData(id++, 1, 0, 2));
            board.SetBlock(2, 0, new BlockData(id++, 2, 0, 3));
            board.SetBlock(0, 1, new BlockData(id++, 0, 1, 4));
            board.SetBlock(1, 1, new BlockData(id++, 1, 1, 1));
            board.SetBlock(2, 1, new BlockData(id++, 2, 1, 2));
            board.SetBlock(0, 2, new BlockData(id++, 0, 2, 3));
            board.SetBlock(1, 2, new BlockData(id++, 1, 2, 4));
            board.SetBlock(2, 2, new BlockData(id++, 2, 2, 1));

            Assert.IsTrue(mechanics.IsDeadlocked());
        }

        [Test]
        public void IsDeadlocked_PairsOnlyWithMinGroupSize3_ReturnsTrue()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);

            int[,] colors = { { 1, 1, 2 }, { 3, 4, 2 }, { 3, 4, 1 } };
            int id = 1;
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    board.SetBlock(x, y, new BlockData(id++, x, y, colors[y, x]));

            Assert.IsFalse(mechanics.IsDeadlocked(2));
            Assert.IsTrue(mechanics.IsDeadlocked(3));
        }

        [Test]
        public void IsDeadlocked_TwoAdjacentSameColorHorizontal_ReturnsFalse()
        {

            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);

            int id = 1;
            board.SetBlock(0, 0, new BlockData(id++, 0, 0, 1));
            board.SetBlock(1, 0, new BlockData(id++, 1, 0, 1));
            board.SetBlock(2, 0, new BlockData(id++, 2, 0, 2));
            board.SetBlock(0, 1, new BlockData(id++, 0, 1, 3));
            board.SetBlock(1, 1, new BlockData(id++, 1, 1, 4));
            board.SetBlock(2, 1, new BlockData(id++, 2, 1, 3));
            board.SetBlock(0, 2, new BlockData(id++, 0, 2, 4));
            board.SetBlock(1, 2, new BlockData(id++, 1, 2, 3));
            board.SetBlock(2, 2, new BlockData(id++, 2, 2, 4));

            Assert.IsFalse(mechanics.IsDeadlocked());
        }

        [Test]
        public void IsDeadlocked_TwoAdjacentSameColorVertical_ReturnsFalse()
        {

            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);

            int id = 1;
            board.SetBlock(0, 0, new BlockData(id++, 0, 0, 1));
            board.SetBlock(1, 0, new BlockData(id++, 1, 0, 2));
            board.SetBlock(2, 0, new BlockData(id++, 2, 0, 3));
            board.SetBlock(0, 1, new BlockData(id++, 0, 1, 1));
            board.SetBlock(1, 1, new BlockData(id++, 1, 1, 4));
            board.SetBlock(2, 1, new BlockData(id++, 2, 1, 3));
            board.SetBlock(0, 2, new BlockData(id++, 0, 2, 4));
            board.SetBlock(1, 2, new BlockData(id++, 1, 2, 3));
            board.SetBlock(2, 2, new BlockData(id++, 2, 2, 4));

            Assert.IsFalse(mechanics.IsDeadlocked());
        }

        [Test]
        public void IsDeadlocked_EmptyBoard_ReturnsTrue()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);


            Assert.IsTrue(mechanics.IsDeadlocked());
        }

        [Test]
        public void IsDeadlocked_SingleBlock_ReturnsTrue()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);

            board.SetBlock(1, 1, new BlockData(1, 1, 1, 1));


            Assert.IsTrue(mechanics.IsDeadlocked());
        }

        [Test]
        public void IsDeadlocked_LargeGroupExists_ReturnsFalse()
        {

            var board = new Board(4, 4);
            var mechanics = new BoardMechanics(board);

            int id = 1;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    board.SetBlock(x, y, new BlockData(id++, x, y, (x + y) % 4 + 1));
                }
            }


            board.SetBlock(0, 0, new BlockData(id++, 0, 0, 5));
            board.SetBlock(1, 0, new BlockData(id++, 1, 0, 5));
            board.SetBlock(0, 1, new BlockData(id++, 0, 1, 5));
            board.SetBlock(1, 1, new BlockData(id++, 1, 1, 5));

            Assert.IsFalse(mechanics.IsDeadlocked());
        }

        #endregion

        #region ShuffleBoard Tests

        [Test]
        public void ShuffleBoard_DeadlockedBoard_CreatesValidMatch()
        {

            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);
            var matchFinder = new MatchFinder(board);

            int id = 1;

            board.SetBlock(0, 0, new BlockData(id++, 0, 0, 1));
            board.SetBlock(1, 0, new BlockData(id++, 1, 0, 2));
            board.SetBlock(2, 0, new BlockData(id++, 2, 0, 1));
            board.SetBlock(0, 1, new BlockData(id++, 0, 1, 2));
            board.SetBlock(1, 1, new BlockData(id++, 1, 1, 1));
            board.SetBlock(2, 1, new BlockData(id++, 2, 1, 2));
            board.SetBlock(0, 2, new BlockData(id++, 0, 2, 1));
            board.SetBlock(1, 2, new BlockData(id++, 1, 2, 2));
            board.SetBlock(2, 2, new BlockData(id++, 2, 2, 1));

            Assert.IsTrue(mechanics.IsDeadlocked(), "Board should start deadlocked");

            mechanics.ShuffleBoard(2);

            Assert.IsFalse(mechanics.IsDeadlocked(), "Board should not be deadlocked after shuffle");
        }

        [Test]
        public void ShuffleBoard_SameSeed_ProducesSameLayout()
        {
            Board Build(int seed, out BoardMechanics m)
            {
                var board = new Board(4, 4);
                m = new BoardMechanics(board, seed);
                int id = 1;
                for (int y = 0; y < 4; y++)
                    for (int x = 0; x < 4; x++)
                        board.SetBlock(x, y, new BlockData(id++, x, y, (x + y) % 2 + 1 + (y / 2) * 2));
                return board;
            }

            var a = Build(42, out var ma);
            var b = Build(42, out var mb);
            ma.ShuffleBoard(2);
            mb.ShuffleBoard(2);

            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    Assert.AreEqual(a.GetBlock(x, y).ColorIndex, b.GetBlock(x, y).ColorIndex);
        }

        [Test]
        public void ShuffleBoard_PreservesBlockCount()
        {
            var board = new Board(4, 4);
            var mechanics = new BoardMechanics(board);

            int id = 1;
            int initialCount = 0;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    board.SetBlock(x, y, new BlockData(id++, x, y, (x + y) % 3 + 1));
                    initialCount++;
                }
            }

            mechanics.ShuffleBoard(2);

            int finalCount = 0;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (board.GetBlock(x, y).IsValid)
                        finalCount++;
                }
            }

            Assert.AreEqual(initialCount, finalCount, "Shuffle should preserve block count");
        }

        [Test]
        public void ShuffleBoard_PreservesColorDistribution()
        {
            var board = new Board(4, 4);
            var mechanics = new BoardMechanics(board);

            int id = 1;
            int[] colorCountsBefore = new int[4];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    int color = (x + y) % 3 + 1;
                    board.SetBlock(x, y, new BlockData(id++, x, y, color));
                    colorCountsBefore[color]++;
                }
            }

            mechanics.ShuffleBoard(2);

            int[] colorCountsAfter = new int[4];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    var block = board.GetBlock(x, y);
                    if (block.IsValid)
                        colorCountsAfter[block.ColorIndex]++;
                }
            }

            for (int i = 1; i <= 3; i++)
            {
                Assert.AreEqual(colorCountsBefore[i], colorCountsAfter[i], 
                    $"Color {i} count should be preserved after shuffle");
            }
        }

        [Test]
        public void ShuffleBoard_UpdatesBlockPositions()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);

            int id = 1;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    board.SetBlock(x, y, new BlockData(id++, x, y, (x + y) % 2 + 1));
                }
            }

            mechanics.ShuffleBoard(2);


            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var block = board.GetBlock(x, y);
                    if (block.IsValid)
                    {
                        Assert.AreEqual(x, block.X, $"Block at ({x},{y}) should have X={x}");
                        Assert.AreEqual(y, block.Y, $"Block at ({x},{y}) should have Y={y}");
                    }
                }
            }
        }

        #endregion

        #region ApplyGravity Tests

        [Test]
        public void ApplyGravity_BlocksFallDown()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);


            board.SetBlock(0, 0, new BlockData(1, 0, 0, 1));
            board.SetBlock(1, 1, new BlockData(2, 1, 1, 2));
            board.SetBlock(2, 2, new BlockData(3, 2, 2, 3));

            mechanics.ApplyGravity();

            Assert.IsTrue(board.GetBlock(0, 0).IsValid, "Block 1 should stay at (0,0)");
            Assert.IsTrue(board.GetBlock(1, 0).IsValid, "Block 2 should fall to (1,0)");
            Assert.IsTrue(board.GetBlock(2, 0).IsValid, "Block 3 should fall to (2,0)");
            
            Assert.IsFalse(board.GetBlock(1, 1).IsValid, "Position (1,1) should be empty");
            Assert.IsFalse(board.GetBlock(2, 2).IsValid, "Position (2,2) should be empty");
        }

        [Test]
        public void ApplyGravity_UpdatesBlockPositions()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);


            board.SetBlock(1, 2, new BlockData(1, 1, 2, 1));

            mechanics.ApplyGravity();

            var block = board.GetBlock(1, 0);
            Assert.IsTrue(block.IsValid, "Block should have fallen to y=0");
            Assert.AreEqual(1, block.X, "Block X should be 1");
            Assert.AreEqual(0, block.Y, "Block Y should be updated to 0");
        }

        #endregion

        #region Blast Tests

        [Test]
        public void Blast_RemovesSpecifiedBlocks()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);

            var block1 = new BlockData(1, 0, 0, 1);
            var block2 = new BlockData(2, 1, 0, 1);
            var block3 = new BlockData(3, 2, 0, 2);

            board.SetBlock(0, 0, block1);
            board.SetBlock(1, 0, block2);
            board.SetBlock(2, 0, block3);

            mechanics.Blast(new System.Collections.Generic.List<BlockData> { block1, block2 });

            Assert.IsFalse(board.GetBlock(0, 0).IsValid, "Block 1 should be removed");
            Assert.IsFalse(board.GetBlock(1, 0).IsValid, "Block 2 should be removed");
            Assert.IsTrue(board.GetBlock(2, 0).IsValid, "Block 3 should remain");
        }

        [Test]
        public void Blast_VerifiesBlockIdBeforeRemoving()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);

            var originalBlock = new BlockData(1, 0, 0, 1);
            var differentBlock = new BlockData(99, 0, 0, 1);

            board.SetBlock(0, 0, originalBlock);


            mechanics.Blast(new System.Collections.Generic.List<BlockData> { differentBlock });


            Assert.IsTrue(board.GetBlock(0, 0).IsValid, "Block should not be removed if ID doesn't match");
            Assert.AreEqual(1, board.GetBlock(0, 0).Id, "Original block ID should be preserved");
        }

        #endregion

        #region RefillBoard Tests

        [Test]
        public void RefillBoard_FillsEmptySpaces()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);
            mechanics.SetNextBlockId(100);


            board.SetBlock(0, 0, new BlockData(1, 0, 0, 1));
            board.SetBlock(1, 1, new BlockData(2, 1, 1, 2));

            mechanics.RefillBoard(3, 42);


            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Assert.IsTrue(board.GetBlock(x, y).IsValid, 
                        $"Position ({x},{y}) should have a valid block after refill");
                }
            }
        }

        [Test]
        public void RefillBoard_GeneratesUniqueIds()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);
            mechanics.SetNextBlockId(100);

            mechanics.RefillBoard(3, 42);

            var ids = new System.Collections.Generic.HashSet<int>();
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var block = board.GetBlock(x, y);
                    Assert.IsFalse(ids.Contains(block.Id), $"Duplicate ID found: {block.Id}");
                    ids.Add(block.Id);
                }
            }
        }

        [Test]
        public void RefillBoard_DoesNotOverwriteExistingBlocks()
        {
            var board = new Board(3, 3);
            var mechanics = new BoardMechanics(board);
            mechanics.SetNextBlockId(100);

            var existingBlock = new BlockData(1, 1, 1, 5);
            board.SetBlock(1, 1, existingBlock);

            mechanics.RefillBoard(3, 42);

            var blockAfterRefill = board.GetBlock(1, 1);
            Assert.AreEqual(1, blockAfterRefill.Id, "Existing block should not be overwritten");
            Assert.AreEqual(5, blockAfterRefill.ColorIndex, "Existing block color should be preserved");
        }

        #endregion
    }
}
