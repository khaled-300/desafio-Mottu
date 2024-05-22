#!/bin/bash
set -e

# Run the migrations
dotnet ef database update --project MotoRentalService.infrastructure --startup-project MotoRentalService.API