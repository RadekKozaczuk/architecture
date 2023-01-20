#!/bin/bash

DRY_RUN=$1

ECHO='echo '
if [[ "$DRY_RUN" == "false" ]]; then
  ECHO=""
fi

git fetch --unshallow
# git fetch --unshallow gives access to all branches
for branch in $(git branch -a | sed 's/^\s*//' | sed 's/^remotes\///' | grep -v 'main$'); do
  # "git log $branch --since "2 weeks ago"" - logs all commits since two weeks ago
  # line "wc -l" counts lines
  # line "-eq 0" checks if equal zero
  # so generally this whole if checks if the number of commits since two weeks ago is equal to 0
  if [[ "$(git log $branch --since "2 weeks ago" | wc -l)" -eq 0 ]]; then
    local_branch_name=${branch##*/}
    $ECHO git push origin --delete "${local_branch_name}"
  fi
done