using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TennisStatistiques.Api.Entities;
namespace TennisStatistiques.Api.Tests.Helpers
{
    public static class PlayerBuilder
    {
        public static Player Create(
            int id = 1,
            int rank = 1,
            string country = "FRA",
            int weight = 80000,
            int height = 180,
            List<int>? last = null) => new()
            {
                Id = id,
                Firstname = $"Prenom{id}",
                Lastname = $"Nom{id}",
                Shortname = $"P.N{id}",
                Sex = "M",
                Country = new Country { Code = country },
                Data = new PlayerData
                {
                    Rank = rank,
                    Weight = weight,
                    Height = height,
                    Age = 25,
                    Last = last ?? []
                }
            };
    }
}
