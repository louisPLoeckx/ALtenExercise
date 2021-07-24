using ALtenExercise.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ALtenExercise.Repository
{
    public class Repo
    {
        private readonly ILogger _logger;

        public Repo(ILogger<Repo> logger)
        {
            _logger = logger;
        }

        public List<Response> LoadDistribution(LoadInput _loadInput, decimal efficiency, int totalPlants)
        {
            List<Response> responses = new List<Response>();
            Response response = new Response();
            PowerPlant powerPlant1 = new PowerPlant();
            List<PowerPlant> powerPlants = new List<PowerPlant>();
            powerPlants = _loadInput.PowerPlants;
            
            double load = _loadInput.Load;
            int count = 0;
            if (load == 0)
            {
                _logger.LogWarning("Load is empty before use: {name}", load);
            }

            while (count <= totalPlants)
            {
                powerPlant1 = FindHighestEfficienyPlant(powerPlants);
                response = CalculatesLoadPlant(powerPlant1, load, efficiency, _loadInput);
                load = load - response.p;
                responses.Add(response);
                _logger.LogInformation("Response added: {name}", response.Name);
                
                powerPlants.Remove(powerPlant1);
                _logger.LogInformation("PowerPlant Removed: {name}", powerPlant1.Name);
                count++;

            }
            return responses;
        }

        public Response CalculatesLoadPlant(PowerPlant powerPlant, double load, decimal efficiency, LoadInput _loadInput)
        {
            Response response = new Response();
            if (load > powerPlant.Pmin)
            {
                double remainingLoad = RemainingLoadCalculator(powerPlant, load, _loadInput);
                response.Name = powerPlant.Name;
                response.p = ProductionCalculator(powerPlant, _loadInput, load);
            }
            else if (load <= 0)
            {
                response.Name = powerPlant.Name;
                response.p = 0;
            }
            else if (load <= powerPlant.Pmin) 
            {
                double remainingLoad = RemainingLoadCalculator(powerPlant, load, _loadInput);
                response.Name = powerPlant.Name;
                response.p = ProductionCalculator(powerPlant, _loadInput, load);
            }

            if (response == null || response.Name == "")
            {
                _logger.LogWarning("Response is empty: {name}", response.Name);
            }

            return response;
        }

        public double ProductionCalculator(PowerPlant powerPlant, LoadInput _loadInput, double load) 
        {
            string type = powerPlant.Type.ToLower();
            double produced = 0;
            double remainder = load % powerPlant.Pmax;
            if (type == null || type == "")
            {
                _logger.LogWarning("Powerplant has no type : {name}", type);
            }
            switch (type)
            {
                case "windturbine":
                    produced = (powerPlant.Pmax) * (PCalculator(powerPlant, _loadInput)) / 100;
                    return produced;
                case "gasfired":
                    if ( (remainder) >=  1)
                    {
                        if (remainder > powerPlant.Pmax)
                        {
                            produced = powerPlant.Pmax;
                        }
                        else
                        {
                            produced = remainder;
                        }
                    }
                    else
                    {
                        produced = load;
                    }
                    return produced;
                case "turbojet":
                    if ((remainder) >= 1)
                    {
                        produced = powerPlant.Pmax;
                    }
                    else
                    {
                        produced = load;
                    }
                    return produced;
                default:
                    break;
            }
            return produced;
        }

        public double RemainingLoadCalculator(PowerPlant powerPlant, double load, LoadInput _loadInput)
        {
            double produced = ProductionCalculator(powerPlant, _loadInput, load);
            return load - produced;
        }

        public int PCalculator(PowerPlant powerPlant, LoadInput _loadInput)
        {
            int efficiencyValue = 0;
            string type = powerPlant.Type.ToLower();
            switch (type)
            {
                case "windturbine":
                    return efficiencyValue = _loadInput.fuels.Wind / (Int32)(powerPlant.Efficiency);
                case "gasfired":
                    return efficiencyValue = (Int32)(powerPlant.Efficiency);
                case "turbojet":
                    return efficiencyValue = (Int32)(powerPlant.Efficiency);
                default:
                    break;
            }

            if (efficiencyValue == 0)
            {
                _logger.LogWarning("efficiency is zero: {efficiency}", efficiencyValue);
            }

            return efficiencyValue;
        }

        public PowerPlant FindHighestEfficienyPlant(List<PowerPlant> powerPlants)
        {
            PowerPlant powerPlant1 = new PowerPlant();
            foreach (var powerPlant in powerPlants)
            {
                if (powerPlant.Efficiency >= 0)
                {
                    powerPlant1 = powerPlant;
                }
            }

            if (powerPlant1 == null)
            {
                _logger.LogWarning("no powerplant inserted: {powerplant}", powerPlant1);
            }
            return powerPlant1;
        }

    }
}
