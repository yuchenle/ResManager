cmake_minimum_required(VERSION 3.20)
project(ResManager)

# Enable C++ language as a workaround for linker language detection
# Note: CMake's native C# support is limited. Since the actual building is done via dotnet CLI,
# we use CXX here just to satisfy CMake's linker language requirement.
# The actual compilation and linking is handled by the dotnet CLI, not CMake.
enable_language(CXX)

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

# Collect all source files
file(GLOB_RECURSE CS_SOURCES
    "${CMAKE_SOURCE_DIR}/*.cs"
)

file(GLOB_RECURSE XAML_SOURCES
    "${CMAKE_SOURCE_DIR}/*.xaml"
)

# Define the executable target
# Note: For .NET projects, this creates a CMake target that Visual Studio can recognize
# The actual building is still done by the 'build' custom target using dotnet CLI
add_executable(ResManager
    ${CS_SOURCES}
    ${XAML_SOURCES}
    ${CMAKE_SOURCE_DIR}/ResManager.csproj
    ${CMAKE_SOURCE_DIR}/app.ico
)

# Set output directory and linker language for the executable
if(CMAKE_CONFIGURATION_TYPES)
    set_target_properties(ResManager PROPERTIES
        RUNTIME_OUTPUT_DIRECTORY_DEBUG "${OUTPUT_DIR}/Debug"
        RUNTIME_OUTPUT_DIRECTORY_RELEASE "${OUTPUT_DIR}/Release"
        RUNTIME_OUTPUT_DIRECTORY_MINSIZEREL "${OUTPUT_DIR}/MinSizeRel"
        RUNTIME_OUTPUT_DIRECTORY_RELWITHDEBINFO "${OUTPUT_DIR}/RelWithDebInfo"
        LINKER_LANGUAGE CXX
    )
else()
    set_target_properties(ResManager PROPERTIES
        RUNTIME_OUTPUT_DIRECTORY "${OUTPUT_DIR}/${CMAKE_BUILD_TYPE}"
        LINKER_LANGUAGE CXX
    )
endif()

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
        DEPENDS restore ${CS_SOURCES} ${XAML_SOURCES} ${CMAKE_SOURCE_DIR}/ResManager.csproj
        COMMENT "Building ResManager ($<CONFIG>)..."
        BYPRODUCTS "${OUTPUT_DIR}/$<CONFIG>/ResManager.exe"
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
        DEPENDS restore ${CS_SOURCES} ${XAML_SOURCES} ${CMAKE_SOURCE_DIR}/ResManager.csproj
        COMMENT "Building ResManager (${CMAKE_BUILD_TYPE})..."
        BYPRODUCTS "${OUTPUT_DIR}/${CMAKE_BUILD_TYPE}/ResManager.exe"
    )
endif()

# Make ResManager executable depend on the build target
# This ensures that building ResManager triggers the dotnet build
add_dependencies(ResManager build)

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

