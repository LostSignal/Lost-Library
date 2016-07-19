//// //-----------------------------------------------------------------------
//// // <copyright file="LocalStore.cs" company="Lost Siganl LLC">
//// //     Copyright (c) Lost Siganl LLC. All rights reserved.
//// // </copyright>
//// //-----------------------------------------------------------------------
//// 
//// namespace Lost
//// {
////     using System.Collections.Generic;
////     using System.IO;
//// 
////     public abstract class LocalStore<T>
////     {
////         private LinkedList<LocalStoreFile> items = new LinkedList<LocalStoreFile>();
////         private string storeName = null;
////         private bool isDirty = false;
//// 
////         public LocalStore(string storeName)
////         {
////             this.storeName = storeName;
//// 
////             // search the disk for all files that could already exist and just store the 
////         }
//// 
////         public void Add(T item)
////         {
////             this.items.AddLast(new LocalStoreFile(item));
////             this.isDirty = true;
////         }
////         
////         public void Flush()
////         {
////             while (this.items.Count > 0)
////             {
////                 LocalStoreFile fileStore = this.items.First.Value;
//// 
////                 if (fileStore.Item == null)
////                 {
////                     fileStore.Item = this.LoadFromDisk(fileStore.FilePath);
////                 }
//// 
////                 
////                 UnityEngine.Debug.Assert(fileStore)
//// 
////                 if (this.Upload(fileStore.Item))
////                 {
////                     if (fileStore.FilePath != null)
////                     {
////                         File.Delete(fileStore.FilePath);
////                     }
////                     
////                     this.items.Remove(fileStore);
////                 }
////                 else
////                 {
////                     break;
////                 }
////             }
////         }
//// 
////         public void Save()
////         {
////             // go through everything in the link list and if the FileName is null, then save it to disk
////             this.isDirty = false;
//// 
////             foreach (LocalStoreFile fileStore in this.items)
////             {
////                 if (fileStore.FilePath == null)
////                 {
////                     fileStore.FilePath = this.SaveToDisk(fileStore.Item);
////                 }
////             }
////         }
//// 
////         protected abstract bool Upload(T item);
//// 
////         private string SaveToDisk(T item)
////         {
////             // TODO decompress, obfuscate, save to disk and return file path
////             return null;
////         }
//// 
////         private T LoadFromDisk(string fileName)
////         {
////             // TODO load from disk, deobfuscate, decompress and return it
////             return default(T);
////         }
//// 
//// 
////         // if you Item is not null, but FileName is, then it needs to be saved from disk
////         // if Item is null, and FileName isn't, then it needs to be loaded from disk
////         private class LocalStoreFile
////         {
////             public LocalStoreFile(T item)
////             {
////                 this.Item = item;
////             }
//// 
////             public LocalStoreFile(string filePath)
////             {
////                 this.FilePath = filePath;
////             }
//// 
////             public string FilePath { get; set; }
//// 
////             public T Item { get; set; }
////         }
////     }
//// }
//// 
