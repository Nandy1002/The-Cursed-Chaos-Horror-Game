using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace VladislavTsurikov.Extensions.Scripts
{
    public static class NativeArrayExtensions
    {               
        public static unsafe void CopyToFast<T>(this NativeArray<T> nativeArray, T[] array) where T : struct
        {
            if (array == null)
            {
                throw new NullReferenceException(nameof(array) + " is null");
            }

            int nativeArrayLength = nativeArray.Length;
            if (array.Length < nativeArrayLength)
            {
                throw new IndexOutOfRangeException(
                    nameof(array) + " is shorter than " + nameof(nativeArray));
            }

            int byteLength = nativeArray.Length * UnsafeUtility.SizeOf<T>();
            void* managedBuffer = UnsafeUtility.AddressOf(ref array[0]);
            void* nativeBuffer = nativeArray.GetUnsafePtr();
            UnsafeUtility.MemCpy(managedBuffer, nativeBuffer, byteLength);
        }

        public static unsafe void CopyToFast<T>(this NativeSlice<T> nativeSlice, T[] array) where T : struct
        {         
            if (array == null)
            {
                throw new NullReferenceException(nameof(array) + " is null");
            }            
            int nativeArrayLength = nativeSlice.Length;
            if (array.Length < nativeArrayLength)
            {
                throw new IndexOutOfRangeException(
                    nameof(array) + " is shorter than " + nameof(nativeSlice));
            }
            int byteLength = nativeSlice.Length * UnsafeUtility.SizeOf<T>();
            void* managedBuffer = UnsafeUtility.AddressOf(ref array[0]);
            void* nativeBuffer = nativeSlice.GetUnsafePtr();
            UnsafeUtility.MemCpy(managedBuffer, nativeBuffer, byteLength);
        }


        public static unsafe void CopyToFast<T>(this NativeArray<T> nativeArray, T[,,] array) where T : struct
        {
            if (array == null)
            {
                throw new NullReferenceException(nameof(array) + " is null");
            }

            int nativeArrayLength = nativeArray.Length;
            int managedArrayLength = array.GetLength(0) * array.GetLength(1) * array.GetLength(2);
            if (managedArrayLength < nativeArrayLength)
            {
                throw new IndexOutOfRangeException(
                    nameof(array) + " is shorter than " + nameof(nativeArray));
            }

            int byteLength = nativeArray.Length * UnsafeUtility.SizeOf<T>();
            void* managedBuffer = UnsafeUtility.AddressOf(ref array[0,0,0]);
            void* nativeBuffer = nativeArray.GetUnsafePtr();
            UnsafeUtility.MemCpy(managedBuffer, nativeBuffer, byteLength);
        }

        public static unsafe void CopyFromFast<T>(this NativeArray<T> nativeArray,T[,] array) where T : struct
        {
            if (array == null)
            {
                throw new NullReferenceException(nameof(array) + " is null");
            }

            int nativeArrayLength = nativeArray.Length;
            int managedArrayLength = array.GetLength(0) * array.GetLength(1);
            if (managedArrayLength > nativeArrayLength)
            {
                throw new IndexOutOfRangeException(
                    nameof(nativeArray) + " is shorter than " + nameof(array));
            }

            int byteLength = managedArrayLength * UnsafeUtility.SizeOf<T>();
            void* managedBuffer = UnsafeUtility.AddressOf(ref array[0, 0]);
            void* nativeBuffer = nativeArray.GetUnsafePtr();
            UnsafeUtility.MemCpy(nativeBuffer,managedBuffer, byteLength);
        }
        
        public static unsafe void CopyFromFast<T>(this NativeArray<T> nativeArray, T[,,] array) where T : struct
        {
            if (array == null)
            {
                throw new NullReferenceException(nameof(array) + " is null");
            }

            int nativeArrayLength = nativeArray.Length;
            int managedArrayLength = array.GetLength(0) * array.GetLength(1) * array.GetLength(2);
            if (managedArrayLength > nativeArrayLength)
            {
                throw new IndexOutOfRangeException(
                    nameof(nativeArray) + " is shorter than " + nameof(array));
            }

            int byteLength = managedArrayLength * UnsafeUtility.SizeOf<T>();
            void* managedBuffer = UnsafeUtility.AddressOf(ref array[0, 0, 0]);
            void* nativeBuffer = nativeArray.GetUnsafePtr();
            UnsafeUtility.MemCpy(nativeBuffer, managedBuffer, byteLength);
        }

        public static unsafe void CopyFromFast<T>(this NativeArray<T> nativeArray,T[] array) where T : struct
        {
            if (array == null)
            {
                throw new NullReferenceException(nameof(array) + " is null");
            }

            int nativeArrayLength = nativeArray.Length;
            int managedArrayLength = array.GetLength(0);
            if (managedArrayLength > nativeArrayLength)
            {
                throw new IndexOutOfRangeException(
                    nameof(nativeArray) + " is shorter than " + nameof(array));
            }

            int byteLength = managedArrayLength * UnsafeUtility.SizeOf<T>();
            void* managedBuffer = UnsafeUtility.AddressOf(ref array[0]);
            void* nativeBuffer = nativeArray.GetUnsafePtr();

            UnsafeUtility.MemCpy(nativeBuffer,managedBuffer, byteLength);
        }       
        
        public static unsafe void CopyFromFast<T>(this NativeArray<T> nativeArray, List<T> managedList) where T : struct
        {
            if (managedList == null)
            {
                throw new NullReferenceException(nameof(managedList) + " is null");
            }

            int nativeArrayLength = nativeArray.Length;
            int managedListLength = managedList.Count;
            T[] managedInternalArray = managedList.GetInternalArray();
            
            if (managedListLength > nativeArrayLength)
            {
                throw new IndexOutOfRangeException(
                    nameof(nativeArray) + " is shorter than " + nameof(managedInternalArray));
            }

            int byteLength = managedListLength * UnsafeUtility.SizeOf<T>();
            void* managedBuffer = UnsafeUtility.AddressOf(ref managedInternalArray[0]);
            void* nativeBuffer = nativeArray.GetUnsafePtr();

            UnsafeUtility.MemCpy(nativeBuffer, managedBuffer, byteLength);
        }
    }
}