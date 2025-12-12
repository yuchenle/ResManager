cmake_minimum_required(VERSION 3.20)
project(ResManager)

# Find .NET SDK
# Note: find_program automatically searches in PATH environment variable
# We also check common installation directories (avoiding ProgramFiles(x86) syntax issue)
find_program(DOTNET_EXE 
    NAMES dotnet
    PATHS 
        "$ENV{ProgramFiles}/dotnet"
        "$ENV{LOCALAPPDATA}/Microsoft/dotnet"
        "C:/Program Files/dotnet"
    PATH_SUFFIXES ""
    DOC "Path to dotnet executable"
)

# Try ProgramFiles(x86) using a workaround if dotnet not found
if(NOT DOTNET_EXE AND WIN32)
    # Use PowerShell to get ProgramFiles(x86) path safely
    execute_process(
        COMMAND powershell -Command "[Environment]::GetFolderPath('ProgramFilesX86')"
        OUTPUT_VARIABLE PROGRAM_FILES_X86
        OUTPUT_STRIP_TRAILING_WHITESPACE
        ERROR_QUIET
    )
    if(PROGRAM_FILES_X86)
        find_program(DOTNET_EXE 
            NAMES dotnet
            PATHS "${PROGRAM_FILES_X86}/dotnet"
            PATH_SUFFIXES ""
            NO_DEFAULT_PATH
        )
    endif()
endif()

if(NOT DOTNET_EXE)
    message(FATAL_ERROR "Could not find dotnet executable. Please install .NET SDK.")
endif()

message(STATUS "Found dotnet: ${DOTNET_EXE}")

# Get dotnet version
execute_process(
    COMMAND ${DOTNET_EXE} --version
    OUTPUT_VARIABLE DOTNET_VERSION
    OUTPUT_STRIP_TRAILING_WHITESPACE
    ERROR_QUIET
)

if(DOTNET_VERSION)
    message(STATUS "Using .NET SDK version: ${DOTNET_VERSION}")
endif()

# Set project file path
set(PROJECT_FILE "${CMAKE_SOURCE_DIR}/ResManager.csproj")
# Use x64/Configuration structure to match Visual Studio expectations
set(OUTPUT_DIR "${CMAKE_BINARY_DIR}/x64")

# Custom target to restore NuGet packages
add_custom_target(restore
    COMMAND ${DOTNET_EXE} restore ${PROJECT_FILE}
    WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
    COMMENT "Restoring NuGet packages..."
)

# Custom target to build the project
# For multi-config generators (like Visual Studio), use generator expression to respect --config
# For single-config generators, CMAKE_BUILD_TYPE will be used
if(CMAKE_CONFIGURATION_TYPES)
    # Multi-config generator (e.g., Visual Studio)
    # Use generator expression to get the actual build configuration
    add_custom_target(build
        COMMAND ${DOTNET_EXE} build ${PROJECT_FILE} 
            --configuration $<CONFIG>
            --output ${OUTPUT_DIR}/$<CONFIG>
        WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
        DEPENDS restore
        COMMENT "Building ResManager ($<CONFIG>)..."
    )
else()
    # Single-config generator (e.g., Unix Makefiles)
    # Use Release configuration by default if CMAKE_BUILD_TYPE is not set
    if(NOT CMAKE_BUILD_TYPE)
        set(CMAKE_BUILD_TYPE "Release")
    endif()
    
    add_custom_target(build
        COMMAND ${DOTNET_EXE} build ${PROJECT_FILE} 
            --configuration ${CMAKE_BUILD_TYPE}
            --output ${OUTPUT_DIR}/${CMAKE_BUILD_TYPE}
        WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
        DEPENDS restore
        COMMENT "Building ResManager (${CMAKE_BUILD_TYPE})..."
    )
endif()

# Custom target for running the application
# This target does NOT execute the application during build - it only ensures the build target runs
# Visual Studio will use VS_DEBUGGER_COMMAND property to run the executable when you click Start Debugging
if(CMAKE_CONFIGURATION_TYPES)
    # Multi-config generator (e.g., Visual Studio)
    # Use CMake's echo utility as a no-op - this ensures build completes without running the app
    add_custom_target(run
        COMMAND ${CMAKE_COMMAND} -E echo "Build complete. Use Visual Studio Start Debugging to run."
        DEPENDS build
        COMMENT "Run target - use Visual Studio Start Debugging (green triangle) to execute"
    )
    
    # Generate .vcxproj.user file to configure Visual Studio debugger settings
    # This tells Visual Studio what executable to run when you click Start Debugging
    # Use absolute paths to ensure Visual Studio can find the executable
    get_filename_component(DEBUG_EXE_PATH "${CMAKE_BINARY_DIR}/x64/Debug/ResManager.exe" ABSOLUTE)
    get_filename_component(RELEASE_EXE_PATH "${CMAKE_BINARY_DIR}/x64/Release/ResManager.exe" ABSOLUTE)
    get_filename_component(MINSIZEREL_EXE_PATH "${CMAKE_BINARY_DIR}/x64/MinSizeRel/ResManager.exe" ABSOLUTE)
    get_filename_component(RELWITHDEBINFO_EXE_PATH "${CMAKE_BINARY_DIR}/x64/RelWithDebInfo/ResManager.exe" ABSOLUTE)
    get_filename_component(WORKING_DIR "${CMAKE_SOURCE_DIR}" ABSOLUTE)
    
    # Convert paths to Windows format with backslashes for the XML file
    file(TO_NATIVE_PATH "${DEBUG_EXE_PATH}" DEBUG_EXE_PATH_NATIVE)
    file(TO_NATIVE_PATH "${RELEASE_EXE_PATH}" RELEASE_EXE_PATH_NATIVE)
    file(TO_NATIVE_PATH "${MINSIZEREL_EXE_PATH}" MINSIZEREL_EXE_PATH_NATIVE)
    file(TO_NATIVE_PATH "${RELWITHDEBINFO_EXE_PATH}" RELWITHDEBINFO_EXE_PATH_NATIVE)
    file(TO_NATIVE_PATH "${WORKING_DIR}" WORKING_DIR_NATIVE)
    
    file(GENERATE OUTPUT "${CMAKE_BINARY_DIR}/run.vcxproj.user"
        CONTENT "<?xml version=\"1.0\" encoding=\"utf-8\"?>
<Project ToolsVersion=\"Current\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">
  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Debug|x64'\">
    <LocalDebuggerCommand>${DEBUG_EXE_PATH_NATIVE}</LocalDebuggerCommand>
    <LocalDebuggerWorkingDirectory>${WORKING_DIR_NATIVE}</LocalDebuggerWorkingDirectory>
    <DebuggerFlavor>WindowsLocalDebugger</DebuggerFlavor>
  </PropertyGroup>
  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Release|x64'\">
    <LocalDebuggerCommand>${RELEASE_EXE_PATH_NATIVE}</LocalDebuggerCommand>
    <LocalDebuggerWorkingDirectory>${WORKING_DIR_NATIVE}</LocalDebuggerWorkingDirectory>
    <DebuggerFlavor>WindowsLocalDebugger</DebuggerFlavor>
  </PropertyGroup>
  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='MinSizeRel|x64'\">
    <LocalDebuggerCommand>${MINSIZEREL_EXE_PATH_NATIVE}</LocalDebuggerCommand>
    <LocalDebuggerWorkingDirectory>${WORKING_DIR_NATIVE}</LocalDebuggerWorkingDirectory>
    <DebuggerFlavor>WindowsLocalDebugger</DebuggerFlavor>
  </PropertyGroup>
  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='RelWithDebInfo|x64'\">
    <LocalDebuggerCommand>${RELWITHDEBINFO_EXE_PATH_NATIVE}</LocalDebuggerCommand>
    <LocalDebuggerWorkingDirectory>${WORKING_DIR_NATIVE}</LocalDebuggerWorkingDirectory>
    <DebuggerFlavor>WindowsLocalDebugger</DebuggerFlavor>
  </PropertyGroup>
</Project>"
    )
else()
    # Single-config generator (e.g., Unix Makefiles)
    add_custom_target(run
        # For non-Visual Studio generators, we can still execute directly
        COMMAND "${OUTPUT_DIR}/${CMAKE_BUILD_TYPE}/ResManager.exe"
        WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
        DEPENDS build
        COMMENT "Running ResManager (${CMAKE_BUILD_TYPE})..."
    )
endif()

# Custom target to clean build artifacts
add_custom_target(clean-build
    COMMAND ${DOTNET_EXE} clean ${PROJECT_FILE}
    WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
    COMMENT "Cleaning build artifacts..."
)

# Set default target
set(CMAKE_DEFAULT_TARGET build)

# Note: This CMakeLists.txt uses custom targets to call dotnet CLI.
# CMake's native C# support is limited, so we use dotnet commands instead.
#
# Usage:
#   cmake -B build
#   cmake --build build          # Builds the project
#   cmake --build build --target run    # Builds and runs
#   cmake --build build --target restore # Restores packages
#   cmake --build build --target clean-build # Cleans build
#
# Or use dotnet directly (recommended):
#   dotnet restore
#   dotnet build
#   dotnet run

