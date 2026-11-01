namespace TennisStatistiques.Api.Exceptions
{
    public class NoPlayersException() 
        : Exception("Aucun joueur disponible pour calculer les statistiques.");
}
