namespace SmartFoodPlanner.Services {
  public class AIServiceException : Exception {
    public AIServiceException(string message, Exception innerException) : base(message, innerException) {}
  }
}
