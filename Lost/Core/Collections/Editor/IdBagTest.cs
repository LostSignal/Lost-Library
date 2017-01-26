//-----------------------------------------------------------------------
// <copyright file="IdBagTest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using NUnit.Framework;

    public class IdBagTest
    {
        [Test]
        public void DuplicateAdd()
        {
            IdBag idBag = new IdBag();
            idBag.AddId(0);
            idBag.AddId(0);
            idBag.AddId(0);
            idBag.AddId(0);
            idBag.AddId(0);

            Assert.True(idBag.Count == 1);
            Assert.True(idBag.BitsListCount == 1);
            Assert.True(idBag.ContainsId(0));
        }

        [Test]
        public void InternalBitsCount()
        {
            IdBag idBag = new IdBag();
            Assert.True(idBag.BitsListCount == 0);
            
            idBag.AddId(0);
            Assert.True(idBag.BitsListCount == 1);

            idBag.AddId(31);
            Assert.True(idBag.BitsListCount == 1);

            idBag.AddId(32);
            Assert.True(idBag.BitsListCount == 2);

            idBag.AddId(64);
            Assert.True(idBag.BitsListCount == 3);
        }
        
        [Test]
        public void InternalCalculatedBitCount()
        {
            IdBag idBag = new IdBag();
            Assert.True(idBag.CalculatedBitCount == 0);

            idBag.AddId(0);
            Assert.True(idBag.CalculatedBitCount == 1);

            idBag.AddId(1);
            Assert.True(idBag.CalculatedBitCount == 2);
            
            idBag.AddId(100);
            Assert.True(idBag.CalculatedBitCount == 3);
        }

        [Test]
        public void AddRemoveCount()
        {
            IdBag idBag = new IdBag();
            Assert.True(idBag.Count == 0);
            
            idBag.AddId(0);
            Assert.True(idBag.Count == 1);
            Assert.True(idBag.ContainsId(0));

            idBag.AddId(1);
            Assert.True(idBag.Count == 2);
            Assert.True(idBag.ContainsId(1));

            idBag.AddId(2);
            Assert.True(idBag.Count == 3);
            Assert.True(idBag.ContainsId(2));

            idBag.AddId(3);
            Assert.True(idBag.Count == 4);
            Assert.True(idBag.ContainsId(3));

            idBag.AddId(4);
            Assert.True(idBag.Count == 5);
            Assert.True(idBag.ContainsId(4));

            idBag.AddId(5);
            Assert.True(idBag.Count == 6);
            Assert.True(idBag.ContainsId(5));

            idBag.RemoveId(5);
            Assert.True(idBag.Count == 5);
            Assert.False(idBag.ContainsId(5));

            idBag.RemoveId(4);
            Assert.True(idBag.Count == 4);
            Assert.False(idBag.ContainsId(4));

            idBag.RemoveId(3);
            Assert.True(idBag.Count == 3);
            Assert.False(idBag.ContainsId(3));

            idBag.RemoveId(2);
            Assert.True(idBag.Count == 2);
            Assert.False(idBag.ContainsId(2));

            idBag.RemoveId(1);
            Assert.True(idBag.Count == 1);
            Assert.False(idBag.ContainsId(1));
            
            idBag.RemoveId(0);
            Assert.True(idBag.Count == 0);
            Assert.False(idBag.ContainsId(0));

            // testing high number
            idBag.AddId(200);
            Assert.True(idBag.Count == 1);
            Assert.True(idBag.ContainsId(200));

            idBag.RemoveId(200);
            Assert.True(idBag.Count == 0);
            Assert.False(idBag.ContainsId(200));
        }

        [Test]
        public void AddIds()
        {
            IdBag idBag = new IdBag();
            idBag.AddIds(0, 1, 2, 3, 4, 5);

            Assert.True(idBag.ContainsId(0));
            Assert.True(idBag.ContainsId(1));
            Assert.True(idBag.ContainsId(2));
            Assert.True(idBag.ContainsId(3));
            Assert.True(idBag.ContainsId(4));
            Assert.True(idBag.ContainsId(5));
            Assert.False(idBag.ContainsId(6));

            Assert.True(idBag.Count == 6);
        }

        [Test]
        public void AddingNullId()
        {
            IdBag bag = new IdBag();
            Assert.Throws<ArgumentException>(delegate { bag.AddId(IdBag.NullId); });
        }

        [Test]
        public void AddingNegative()
        {
            IdBag bag = new IdBag();
            Assert.Throws<ArgumentException>(delegate { bag.AddId(-1); });
        }
    }
}
