﻿using SickDev.CommandSystem;

namespace SickDev.WebRcon {
    static class BuiltInCommands {
        [Command(alias = "echotest", description = "echoes the text back to the console")]
        static string Echo(string message) {
            return message;
        }
    }
}