﻿using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _context;
        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {

            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);

        }


        public LibraryAsset GetById(int id)
        {
            return
            GetAll()
                .FirstOrDefault(asset => asset.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }
            else return "";
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).ISBN;
            }
            else return "";
        }

        public string GetTitle(int id)
        {
            return _context.Books.FirstOrDefault(book => book.Id == id).Title;
        }

        public string GetType(int id)
        {
            var book = _context.LibraryAssets.OfType<Book>()
                .Where(b => b.Id == id);
            return book.Any() ? "Book" : "Video";
        }

        public string GetAuthorOrDirector(int assetId)
        {
            var asset = _context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);
            if(asset==null)
            {
                throw new Exception("Böyle bir asset yok!");        
            }

            var book = asset as Book;

            if (book!=null)
            {
                return book.Author;
            }
            var video = asset as Video;

            if (video != null)
            {
                return video.Director;
            }
            throw new Exception("Tanımlanamadı.");
            //var isBook = _context.LibraryAssets.OfType<Book>()
            //    .Where(asset => asset.Id == assetId).Any();

            //var isVideo = _context.LibraryAssets.OfType<Video>()
            //    .Where(asset => asset.Id == assetId).Any();

            //return isBook ?
            //    _context.Books.FirstOrDefault(book => book.Id == assetId).Author :
            //    _context.Videos.FirstOrDefault(video => video.Id == assetId).Director
            //    ?? "Unknown";
        }
    }
}
