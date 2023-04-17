namespace CCFinal.CanvasIntegration;

public static class Helpers {
    public static DateTime MaxDateTime(this DateTime dateTime, DateTime otherDateTime) {
        return dateTime > otherDateTime
            ? dateTime
            : otherDateTime;
    }
}