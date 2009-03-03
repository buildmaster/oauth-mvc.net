//
//  ApplicationController.h
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "PreferenceController.h"

extern NSString * const OACConsumerKey;
extern NSString * const OACConsumerSecret;
extern NSString * const OACRequestTokenKey;
extern NSString * const OACRequestTokenSecret;
extern NSString * const OACAccessTokenKey;
extern NSString * const OACAccessTokenSecret;
extern NSString * const OACRequestTokenUrl;
extern NSString * const OACRequestTokenAuthUrl;
extern NSString * const OACAccessTokenUrl;
extern NSString * const OACLastRequestUrl;

@interface ApplicationController : NSObject {
	PreferenceController *preferenceController;

}
-(IBAction) showPreferenceController:(id)sender;
@end
