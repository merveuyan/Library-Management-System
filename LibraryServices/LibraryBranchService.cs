﻿using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class LibraryBranchService : ILibraryBranch
    {
        private LibraryContext _context;

        public LibraryBranchService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            return GetAll()
                .FirstOrDefault(b => b.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                .Include(b => b.Patrons)
                .Include(b => b.LibraryAssets);
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return _context
                .LibraryBranches
                .Include(b => b.LibraryAssets)
                .FirstOrDefault(b => b.Id == branchId)
                .LibraryAssets;
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours.Where(h => h.Branch.Id == branchId);
            return DataHelpers.HumanizeBizHours(hours);
        }

        public IEnumerable<string> GetNameOfPatrons(int branchId)
        {
            var names = _context.Patrons
                .Include(p=>p.HomeLibraryBranch)
                .Where(h => h.HomeLibraryBranch.Id == branchId);
            return DataHelpers.HumanizeNames(names);
        }

        public IEnumerable<string> GetNameOfAssets(int branchId)
        {
            var assets = _context.LibraryAssets
                .Where(h => h.Location.Id == branchId);
            return DataHelpers.HumanizeAssets(assets);
        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return _context.LibraryBranches
                .Include(b => b.Patrons)
                .FirstOrDefault(b => b.Id == branchId)
                .Patrons;
        }

        public bool IsBranchOpen(int branchId)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;
            var hours = _context.BranchHours.Where(h => h.Branch.Id == branchId);
            var daysHours = hours.FirstOrDefault(h => h.DayOfWeek == currentDayOfWeek);

            return currentTimeHour < daysHours.CloseTime && currentTimeHour > daysHours.OpenTime;
        }
    }
}
