@echo off
cls

dotnet restore build.proj
dotnet tool install fake-cli --tool-path .fake
IF NOT EXIST build.fsx (
  .fake\fake run init.fsx
)
fake build %*