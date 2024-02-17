#!/bin/bash

target="$1"

if [ "$target" = "tests" ]; then
    dotnet test

elif [ "$target" = "mac" ]; then
    dotnet run --project sandbox/maui --framework net8.0-maccatalyst

elif [ "$target" = "windows" ]; then
    echo "Not implemented."

elif [ "$target" = "ios" ]; then
    echo "Not implemented."
    # Add your iOS build commands here

elif [ "$target" = "android" ]; then
    echo "Not implmented."
    # Add your Android build commands here

elif [ "$target" = "console" ]; then
    dotnet run --project sandbox/console

else
    echo "Invalid target: $target"
    exit 1
fi
