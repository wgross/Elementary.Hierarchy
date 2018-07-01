//using System.Linq;
//using Xunit;

//namespace Elementary.Hierarchy.Reflection.Test
//{
//    public class TraversePropertiesWithDelegatesTest
//    {
//        public class AccessorLevelTestData
//        {
//            public int Readable { get; }
//            private int PrivateReadable { get; }
//            protected int ProtectedReadable { get; }
//            internal int InternalReadable { get; }
//            protected internal int ProtectedInternalReadable { get; }

//            public int ReadWrite { get; set; }

//            private int writable = 0;
//            public int Writable { set { this.writable = value; } }
//        }

//        [Fact]
//        public void Traverse_all_public_readable_properties()
//        {
//            // ACT

//            var result = new AccessorLevelTestData().TraverseProperties();

//            // ASSERT

//            Assert.NotNull(result);
//            Assert.Equal(3, result.Count());
//            Assert.Equal(new[] { nameof(AccessorLevelTestData.Readable), nameof(AccessorLevelTestData.ReadWrite), nameof(AccessorLevelTestData.Writable) }, result.Select(pi => pi.Item2.Name));
//        }

//        public class DeepTestData
//        {
//            public int Property { get; set; }

//            public DeepTestData TestData { get; set; }
//        }

//        [Fact]
//        public void Traverse_all_public_readable_properties_deep()
//        {
//            // ACT

//            var result = new DeepTestData
//            {
//                Property = 1,
//                TestData = new DeepTestData
//                {
//                    Property = 2
//                }
//            }.TraverseProperties();

//            // ASSERT

//            Assert.NotNull(result);
//            Assert.Equal(4, result.Count());
//            Assert.Equal(new[] { nameof(DeepTestData.Property), nameof(DeepTestData.TestData), nameof(DeepTestData.Property), nameof(DeepTestData.TestData)}, result.Select(pi => pi.Item2.Name));
//        }
//    }
//}