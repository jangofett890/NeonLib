using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonLib.Gameplay.MicroGames {
    public static class MicroGamesInputHelper {
        // Function to check if a target value is within a certain range of another value.
        public static bool IsValueInRange(float currentValue, float targetValue, float range) {
            return Mathf.Abs(currentValue - targetValue) <= range;
        }

        // Function to generate a random float value within a specified range.
        public static float GetRandomValueWithinRange(float minValue, float maxValue) {
            return Random.Range(minValue, maxValue);
        }

        // Function to calculate the difficulty factor based on the difficulty level.
        public static float CalculateDifficultyFactor(int difficultyLevel, float baseFactor) {
            return difficultyLevel * baseFactor;
        }

        // Function to calculate the remaining time in a countdown timer.
        public static float CalculateRemainingTime(float startTime, float duration) {
            return Mathf.Max(0, startTime + duration - Time.time);
        }

        // Function to calculate the score multiplier based on streak and combo.
        public static float CalculateScoreMultiplier(int streak, int combo, float baseMultiplier) {
            return streak * combo * baseMultiplier;
        }

        // Function to evaluate a timing-based input event (like QTE) by comparing the input time to the target time.
        public static bool EvaluateTimingEvent(float inputTime, float targetTime, float tolerance) {
            return Mathf.Abs(inputTime - targetTime) <= tolerance;
        }

        // Add more utility functions for common game patterns here
    }
}