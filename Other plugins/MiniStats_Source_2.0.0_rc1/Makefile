#(C)2004-2006 SourceMM Development Team
# Makefile written by David "BAILOPAN" Anderson

#HL2SDK = ../sdk/HL2Sdk
#SMM_ROOT = ../sdk/sourcemm_1.4
#SM_ROOT = ../sdk/sourcemod
#MySQL = ../sdk/MySQL/include
#SRCDS = ../srcds

HL2SDK = ../sdk/HL2Sdk-ob
SMM_ROOT = ../sdk/sourcemm_1.6
SM_ROOT = ../sdk/sourcemod
MySQL = ../sdk/MySQL/include
SRCDS = ../srcds

### EDIT BELOW FOR OTHER PROJECTS ###

OPT_FLAGS = -O3 -fno-rtti -funroll-loops -s -pipe
DEBUG_FLAGS = -g -ggdb3
CPP = ../opt/crosstool/gcc-3.4.1-glibc-2.3.2/i686-unknown-linux-gnu/bin/i686-unknown-linux-gnu-gcc
#CPP = /opt/crosstool/gcc-3.4.1-glibc-2.3.2/i686-unknown-linux-gnu/bin/i686-unknown-linux-gnu-gcc

BINARY = MiniStats_mm_i486.so


OBJECTS = MiniStatsCore.cpp EventListeners.cpp cvars.cpp WeaponList.cpp SrvConsoleCmds.cpp Stats.cpp SteamIDList.cpp BATMenu.cpp ModInfo.cpp StrUtils.cpp EngineUtils.cpp  HLXMenu.cpp \
	recipientfilters.cpp \

LINK = $(HL2SDK)/linux_sdk/tier1_i486.a tier0_i486.so vstdlib_i486.so

HL2PUB = $(HL2SDK)/public

INCLUDE = -I. -I$(HL2PUB) -I$(HL2PUB)/dlls -I$(HL2PUB)/engine -I$(HL2PUB)/tier0 -I$(HL2PUB)/tier1 \
-I$(HL2PUB)/vstdlib -I$(HL2SDK)/tier1  -I$(HL2SDK)/game_shared -I$(HL2SDK)/dlls -I$(HL2SDK)/game/shared -I$(HL2SDK)/public/game/server \
-I$(SMM_ROOT) -I$(SMM_ROOT)/sourcehook  -I$(SMM_ROOT)/sourcemm -I$(MySQL)  \
-I$(SM_ROOT) -I$(SM_ROOT)/sourcepawn -I$(SM_ROOT)/sample_ext/sdk -I$(SM_ROOT)/sample_ext

ifeq "$(DEBUG)" "true"
	BIN_DIR = Debug
	CFLAGS = $(DEBUG_FLAGS)
else
	BIN_DIR = Release
	CFLAGS = $(OPT_FLAGS)
endif

CFLAGS += -fpermissive -D_LINUX -DNDEBUG -Dstricmp=strcasecmp -D_stricmp=strcasecmp -D_strnicmp=strncasecmp -Dstrnicmp=strncasecmp -D_snprintf=snprintf -D_vsnprintf=vsnprintf -D_alloca=alloca -Dstrcmpi=strcasecmp -fPIC -Wno-deprecated -fno-rtti -msse

OBJ_LINUX := $(OBJECTS:%.cpp=$(BIN_DIR)/%.o)

$(BIN_DIR)/%.o: %.cpp
	$(CPP) $(INCLUDE) $(CFLAGS) -o $@ -c $<

all:
	mkdir -p $(BIN_DIR)
	ln -sf $(SRCDS)/bin/vstdlib_i486.so vstdlib_i486.so
	ln -sf $(SRCDS)/bin/tier0_i486.so tier0_i486.so
	$(MAKE) sourcemm

sourcemm: $(OBJ_LINUX)
	$(CPP) $(INCLUDE) $(CFLAGS) $(OBJ_LINUX) $(LINK) -shared -ldl -lm -o$(BIN_DIR)/$(BINARY)

debug:	
	$(MAKE) all DEBUG=true

default: all

clean:
	rm -rf Release/*.o
	rm -rf Release/$(BINARY)
	rm -rf Debug/*.o
	rm -rf Debug/$(BINARY)
