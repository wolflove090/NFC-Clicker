#import <Foundation/Foundation.h>
 
extern "C" {
    int getThermalState() {
        return (int) [NSProcessInfo.processInfo thermalState];
    }
}
