BUILD ?= Debug
#VERBOSE = @echo "=== target: $@"

all: transcelerator

transcelerator:
	xbuild Transcelerator.sln /target:Transcelerator /property:Configuration=$(BUILD)

install:  install-files 
	$(VERBOSE)


install-directories:
	$(VERBOSE)
	mkdir -p $(DESTDIR)/usr/lib/Paratext/plugins/Transcelerator

install-files: install-directories
	$(VERBOSE)
	cp Transcelerator/bin/$(BUILD)/*.exe                                   $(DESTDIR)/usr/lib/Paratext/plugins/Transcelerator
	cp Transcelerator/bin/$(BUILD)/*.dll                                     $(DESTDIR)/usr/lib/Paratext/plugins/Transcelerator
	cp Transcelerator/bin/$(BUILD)/*.ico                                     $(DESTDIR)/usr/lib/Paratext/plugins/Transcelerator
	cp Transcelerator/bin/$(BUILD)/*.xml                                     $(DESTDIR)/usr/lib/Paratext/plugins/Transcelerator
	cp Transcelerator/bin/$(BUILD)/*.htm                                     $(DESTDIR)/usr/lib/Paratext/plugins/Transcelerator
	cp Transcelerator/bin/$(BUILD)/*.xsd                                     $(DESTDIR)/usr/lib/Paratext/plugins/Transcelerator
	cp Transcelerator/bin/$(BUILD)/*.config                                     $(DESTDIR)/usr/lib/Paratext/plugins/Transcelerator
	
