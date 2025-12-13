cmake_minimum_required(VERSION 3.20)
project(ResManager)

# CMake does NOT truly support C#
# We enable CXX only to satisfy linker language requirements
enable_language(CXX)

# ------------------------------------------------------------
# Locate dotnet
# ------------------------------------------------------------
find_program(DOTNET_EXE
    NAMES dotnet
    PATHS
        "$ENV{ProgramFiles}/dotnet"
        "$ENV{LOCALAPPDATA}/Microsoft/dotnet"
        "C:/Program Files/dotnet"
)

if(NOT DOTNET_EXE AND WIN32)
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
            NO_DEFAULT_PATH
        )
    endif()
endif()

if(NOT DOTNET_EXE)
    message(FATAL_ERROR "dotnet SDK not found.")
endif()

message(STATUS "Found dotnet: ${DOTNET_EXE}")

# ------------------------------------------------------------
# Paths
# ------------------------------------------------------------
set(PROJECT_FILE "${CMAKE_SOURCE_DIR}/ResManager.csproj")
set(OUTPUT_DIR "${CMAKE_BINARY_DIR}/x64")

# ------------------------------------------------------------
# Sources (for Visual Studio solution visibility only)
# ------------------------------------------------------------
file(GLOB_RECURSE CS_SOURCES "*.cs")
file(GLOB_RECURSE XAML_SOURCES "*.xaml")

# ------------------------------------------------------------
# Executable target (VS needs this!)
# ------------------------------------------------------------
add_executable(ResManager
    ${CS_SOURCES}
    ${XAML_SOURCES}
    ${PROJECT_FILE}
    app.ico
)

set_target_properties(ResManager PROPERTIES
    LINKER_LANGUAGE CXX
)

# ------------------------------------------------------------
# Output paths per configuration
# ------------------------------------------------------------
if(CMAKE_CONFIGURATION_TYPES)
    foreach(cfg Debug Release RelWithDebInfo MinSizeRel)
        set_target_properties(ResManager PROPERTIES
            RUNTIME_OUTPUT_DIRECTORY_${cfg} "${OUTPUT_DIR}/${cfg}"
        )
    endforeach()
endif()

# ------------------------------------------------------------
# dotnet restore
# ------------------------------------------------------------
add_custom_target(dotnet_restore
    COMMAND ${DOTNET_EXE} restore ${PROJECT_FILE}
    WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
)

# ------------------------------------------------------------
# dotnet build (configuration-aware)
# ------------------------------------------------------------
add_custom_target(dotnet_build
    COMMAND ${DOTNET_EXE} build ${PROJECT_FILE}
        --configuration $<CONFIG>
        --output ${OUTPUT_DIR}/$<CONFIG>
    WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
    DEPENDS dotnet_restore
    BYPRODUCTS "${OUTPUT_DIR}/$<CONFIG>/ResManager.exe"
)

# Ensure VS "Build" builds the .NET app
add_dependencies(ResManager dotnet_build)

# ------------------------------------------------------------
# Visual Studio debugger integration (THIS IS THE IMPORTANT PART)
# ------------------------------------------------------------
if(MSVC)
    set_target_properties(ResManager PROPERTIES
        VS_DEBUGGER_COMMAND
            "${OUTPUT_DIR}/$<CONFIG>/ResManager.exe"
        VS_DEBUGGER_WORKING_DIRECTORY
            "${OUTPUT_DIR}/$<CONFIG>"
    )
endif()
