﻿namespace Tartaros.Economy
{
    using System.Collections.Generic;
    public interface ISectorResourcesWallet
    {
        Dictionary<SectorRessourceType, int> resourcesAmount { get; }

        int GetAmount(SectorRessourceType ressource);
        void AddAmount(SectorRessourceType ressource, int amount);
        void RemoveAmount(SectorRessourceType ressource, int amount);
    }
}