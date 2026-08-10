# GitToVsts

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](LICENSE)

## Requirements

- a current version of git for windows
- a current version of git credential manager for windows (maybe included in git for windows)
- you maybe have to have been connected with the target Azure DevOps once to get git credential manager working fine

## Open Items

- logging is not implemented
- cleanup code
- two factor authentication for git
- encryption of credentials
- maybe the whole process should be revised

## Package Feeds

Default by `NuGet.config` is myget.org

| Feed                           | Feed Url                                                         |
| :----------------------------- | :--------------------------------------------------------------- |
| ![myget.org][myGetBadge]       | <https://www.myget.org/F/evilbaschdi/api/v3/index.json>          |
| ![codeberg.org][codebergBadge] | <https://codeberg.org/api/packages/evilbaschdi/nuget/index.json> |

## Quality & Activity

| Branch                          | Status & Activity                                                                                                                                      |
| :------------------------------ | :----------------------------------------------------------------------------------------------------------------------------------------------------- |
| ![Main Branch][mainBranchBadge] | [![CodeFactor][codeFactorMainBadge]][codeFactorMainOverview] ![Commit Activity Main][commitActivityMainBadge] ![Last Commit Main][lastCommitMainBadge] |

[myGetBadge]: https://img.shields.io/badge/MyGet.org-gray?style=for-the-badge&logo=myget
[codebergBadge]: https://img.shields.io/badge/Codeberg-gray?style=for-the-badge&logo=codeberg

[mainBranchBadge]: https://img.shields.io/badge/branch-main-brightgreen?style=for-the-badge&logo=git&logoColor=white&color=c9ff00
[developBranchBadge]: https://img.shields.io/badge/branch-develop-blue?style=for-the-badge&logo=git&logoColor=white&color=0080ff

[codeFactorMainBadge]: https://www.codefactor.io/repository/github/evilbaschdi/GitToVsts/badge/main?style=for-the-badge
[codeFactorMainOverview]: https://www.codefactor.io/repository/github/evilbaschdi/GitToVsts/overview/main
[commitActivityMainBadge]: https://img.shields.io/github/commit-activity/m/evilbaschdi/GitToVsts/main?style=for-the-badge
[lastCommitMainBadge]: https://img.shields.io/github/last-commit/evilbaschdi/GitToVsts/main?style=for-the-badge

[codeFactorDevelopBadge]: https://www.codefactor.io/repository/github/evilbaschdi/GitToVsts/badge/develop?style=for-the-badge
[codeFactorDevelopOverview]: https://www.codefactor.io/repository/github/evilbaschdi/GitToVsts/overview/develop
[commitActivityDevelopBadge]: https://img.shields.io/github/commit-activity/m/evilbaschdi/GitToVsts/develop?style=for-the-badge
[lastCommitDevelopBadge]: https://img.shields.io/github/last-commit/evilbaschdi/GitToVsts/develop?style=for-the-badge