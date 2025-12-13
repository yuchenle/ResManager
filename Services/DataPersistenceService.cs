using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ResManager.Models;

namespace ResManager.Services
{
    public class DataPersistenceService
    {
        private readonly string _dishesFilePath;

        public DataPersistenceService()
        {
            // Store data in AppData\Local\ResManager
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataPath, "ResManager");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _dishesFilePath = Path.Combine(appFolder, "dishes.json");
        }

        public List<Dish> LoadDishes()
        {
            if (!File.Exists(_dishesFilePath))
            {
                return new List<Dish>();
            }

            try
            {
                string json = File.ReadAllText(_dishesFilePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<Dish>();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var dishData = JsonSerializer.Deserialize<List<DishData>>(json, options);
                if (dishData == null)
                {
                    return new List<Dish>();
                }

                return dishData.Select(d => new Dish
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description ?? string.Empty,
                    Price = d.Price,
                    Category = d.Category,
                    IsAvailable = d.IsAvailable
                }).ToList();
            }
            catch (Exception)
            {
                // If loading fails, return empty list
                return new List<Dish>();
            }
        }

        public void SaveDishes(IEnumerable<Dish> dishes)
        {
            try
            {
                var dishData = dishes.Select(d => new DishData
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    Category = d.Category,
                    IsAvailable = d.IsAvailable
                }).ToList();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(dishData, options);
                File.WriteAllText(_dishesFilePath, json);
            }
            catch (Exception)
            {
                // Silently fail - could log error in production
            }
        }

        private class DishData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public DishCategory Category { get; set; }
            public bool IsAvailable { get; set; }
        }
    }
}
