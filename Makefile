# (C)2004-2009 Metamod:Source Development Team
# Makefile written by David "BAILOPAN" Anderson

###########################################
### EDIT THESE PATHS FOR YOUR OWN SETUP ###
###########################################

BASE_DIR = /home/dede
HL2SDK_ORIG = $(BASE_DIR)/mercurial/hl2sdk
HL2SDK_OB = $(BASE_DIR)/mercurial/hl2sdk-ob
HL2SDK_OB_VALVE = $(BASE_DIR)/mercurial/hl2sdk-ob-valve
HL2SDK_L4D = $(BASE_DIR)/mercurial/hl2sdk-l4d
HL2SDK_L4D2 = $(BASE_DIR)/mercurial/hl2sdk-l4d2
MMSOURCE18 = $(BASE_DIR)/mercurial/mmsource-1.8
SRCDS_BASE = $(BASE_DIR)/srcds

#####################################
### EDIT BELOW FOR OTHER PROJECTS ###
#####################################

OBJECTS = monoplug.cpp
BINARY = monoplug_i486.so
CSPROJECT = MonoPlug.Managed.csproj MonoPlug.Samples.csproj

##############################################
### CONFIGURE ANY OTHER FLAGS/OPTIONS HERE ###
##############################################

OPT_FLAGS = -O3 -funroll-loops -s -pipe
GCC4_FLAGS = -fvisibility=hidden -fvisibility-inlines-hidden `pkg-config --cflags mono`
DEBUG_FLAGS = -g -ggdb3 -D_DEBUG
CPP = gcc-4.1

XBUILD = xbuild
XB_DEBUG = /property:Configuration=Debug
XB_RELEASE = /property:Configuration=Release
XB_FLAGS = /t:Build /nologo


override ENGSET = false
ifeq "$(ENGINE)" "original"
	HL2SDK = $(HL2SDK_ORIG)
	HL2PUB = $(HL2SDK)/public
	HL2LIB = $(HL2SDK)/linux_sdk
	CFLAGS += -DSOURCE_ENGINE=1
	METAMOD = $(MMSOURCE18)/core-legacy
	INCLUDE += -I$(HL2SDK)/public/dlls
	SRCDS = $(SRCDS_BASE)
	LIB_SUFFIX = i486
	override ENGSET = true
endif
ifeq "$(ENGINE)" "orangebox"
	HL2SDK = $(HL2SDK_OB)
	HL2PUB = $(HL2SDK)/public
	HL2LIB = $(HL2SDK)/lib/linux
	CFLAGS += -DSOURCE_ENGINE=3
	METAMOD = $(MMSOURCE18)/core
	INCLUDE += -I$(HL2SDK)/public/game/server
	SRCDS = $(SRCDS_BASE)/orangebox
	LIB_SUFFIX = i486
	override ENGSET = true
endif
ifeq "$(ENGINE)" "orangeboxvalve"
	HL2SDK = $(HL2SDK_OB_VALVE)
	HL2PUB = $(HL2SDK)/public
	HL2LIB = $(HL2SDK)/lib/linux
	CFLAGS += -DSOURCE_ENGINE=4
	METAMOD = $(MMSOURCE18)/core
	INCLUDE += -I$(HL2SDK)/public/game/server
	SRCDS = $(SRCDS_BASE)/orangebox
	LIB_SUFFIX = i486
	override ENGSET = true
endif
ifeq "$(ENGINE)" "left4dead"
	HL2SDK = $(HL2SDK_L4D)
	HL2PUB = $(HL2SDK)/public
	HL2LIB = $(HL2SDK)/lib/linux
	CFLAGS += -DSOURCE_ENGINE=5
	METAMOD = $(MMSOURCE18)/core
	INCLUDE += -I$(HL2SDK)/public/game/server
	SRCDS = $(SRCDS_BASE)/l4d
	LIB_SUFFIX = i486
	override ENGSET = true
endif
ifeq "$(ENGINE)" "left4dead2"
	HL2SDK = $(HL2SDK_L4D2)
	HL2PUB = $(HL2SDK)/public
	HL2LIB = $(HL2SDK)/lib/linux
	CFLAGS += -DSOURCE_ENGINE=6
	METAMOD = $(MMSOURCE18)/core
	INCLUDE += -I$(HL2SDK)/public/game/server
	SRCDS = $(SRCDS_BASE)/left4dead2_demo
	LIB_SUFFIX = linux
	override ENGSET = true
endif

CFLAGS += -DSE_EPISODEONE=1 -DSE_DARKMESSIAH=2 -DSE_ORANGEBOX=3 -DSE_ORANGEBOXVALVE=4 \
	-DSE_LEFT4DEAD=5 -DSE_LEFT4DEAD2=6

LINK += $(HL2LIB)/tier1_i486.a vstdlib_$(LIB_SUFFIX).so tier0_$(LIB_SUFFIX).so -static-libgcc

INCLUDE += -I. -I.. -I$(HL2PUB) -I$(HL2PUB)/engine -I$(HL2PUB)/mathlib -I$(HL2PUB)/vstdlib \
	-I$(HL2PUB)/tier0 -I$(HL2PUB)/tier1 -I. -I$(METAMOD) -I$(METAMOD)/sourcehook `pkg-config --libs mono`

################################################
### DO NOT EDIT BELOW HERE FOR MOST PROJECTS ###
################################################

ifeq "$(DEBUG)" "true"
	BIN_DIR = Debug.$(ENGINE)
	CFLAGS += $(DEBUG_FLAGS)
	XB_FLAGS += $(XB_DEBUG)
else
	BIN_DIR = Release.$(ENGINE)
	CFLAGS += $(OPT_FLAGS)
	XB_FLAGS += $(XB_RELEASE)
endif

GCC_VERSION := $(shell $(CPP) -dumpversion >&1 | cut -b1)

CFLAGS += -D_LINUX -Dstricmp=strcasecmp -D_stricmp=strcasecmp -D_strnicmp=strncasecmp \
	-Dstrnicmp=strncasecmp -D_snprintf=snprintf -D_vsnprintf=vsnprintf -D_alloca=alloca \
	-Dstrcmpi=strcasecmp -Wall -Wno-non-virtual-dtor -Werror -fPIC -fno-exceptions \
	-fno-rtti -msse -m32 -fno-strict-aliasing

ifeq "$(GCC_VERSION)" "4"
	CFLAGS += $(GCC4_FLAGS)
endif

OBJ_LINUX := $(OBJECTS:%.cpp=$(BIN_DIR)/%.o)

OBJ_CS := $(CSPROJECTS:%.csproj)

$(CSPROJECTS:%.csproj):
	$(XBUILD) $(XB_FLAGS) $<

$(BIN_DIR)/%.o: %.cpp
	$(CPP) $(INCLUDE) $(CFLAGS) -o $@ -c $<

all: check
	mkdir -p $(BIN_DIR)
	ln -sf $(HL2LIB)/vstdlib_$(LIB_SUFFIX).so
	ln -sf $(HL2LIB)/tier0_$(LIB_SUFFIX).so
	$(MAKE) -f Makefile monoplug_native
	$(MAKE) -f Makefile monoplug_managed
	
check:
	if [ "$(ENGSET)" = "false" ]; then \
		echo "You must supply one of the following values for ENGINE:"; \
		echo "left4dead2, left4dead, orangeboxvalve or orangebox"; \
		exit 1; \
	fi

monoplug_native: check $(OBJ_LINUX)
	$(CPP) $(INCLUDE) -m32 $(OBJ_LINUX) $(LINK) -shared -ldl -lm -o$(BIN_DIR)/$(BINARY)

monoplug_managed: check $(OBJ_CS)
	$(XBUILD) $(XB_FLAGS) $(OBJ_CS)
	echo $(OBJ_CS)
	
default: all

clean: check
	rm -rf $(BIN_DIR)/*.o
	rm -rf $(BIN_DIR)/$(BINARY)

