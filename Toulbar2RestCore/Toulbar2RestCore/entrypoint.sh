#!/usr/bin/env bash

set -eo pipefail
shopt -s nullglob

# UID/GID map to unknown user/group, $HOME=/ (the default when no home directory is defined)
eval $( fixuid )
# UID/GID now match user/group, $HOME has been set to user's home directory

exec "$@"
