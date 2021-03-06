############################################################
# Copyright 2020 New Relic Corporation. All rights reserved.
# SPDX-License-Identifier: Apache-2.0
############################################################

param($installPath, $toolsPath, $package, $project)

Import-Module (Join-Path $toolsPath NewRelicHelper.psm1)

Write-Host "***Updating the projects .config file with the NewRelic.AppName***"
update_project_config $project

Write-Host "***Package install is complete***"





