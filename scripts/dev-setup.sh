#!/usr/bin/env bash
set -euo pipefail

if ! command -v dotnet >/dev/null 2>&1; then
	echo "Installing .NET SDK 8 locally..."
	TMP_SCRIPT="/tmp/dotnet-install.sh"
	wget -q https://dot.net/v1/dotnet-install.sh -O "$TMP_SCRIPT"
	chmod +x "$TMP_SCRIPT"
	"$TMP_SCRIPT" --channel 8.0 --quality ga --install-dir "$HOME/dotnet"
	echo "Add to PATH: export PATH=\"$HOME/dotnet:\$PATH\""
else
	echo ".NET already installed: $(dotnet --version)"
fi

echo "Done."
