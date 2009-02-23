//
//  PreferenceController.m
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import "PreferenceController.h"
#import "ApplicationController.h"

@implementation PreferenceController

-(id)init
{
	if(![super initWithWindowNibName:@"Preferences"])
	{
		return nil;
	}
	return self;
}

-(void) windowDidLoad
{
	NSUserDefaults *defaults = 	[NSUserDefaults standardUserDefaults];
	[requestTokenUrl setStringValue:[defaults valueForKey:OACRequestTokenUrl]];
	[requestAuthUrl setStringValue:[defaults valueForKey:OACRequestTokenAuthUrl]];
	[accessTokenUrl setStringValue:[defaults valueForKey:OACAccessTokenUrl]];
	[consumerKey setStringValue:[defaults valueForKey:OACConsumerKey]];
	[consumerSecret setStringValue:[defaults valueForKey:OACConsumerSecret]];
}

-(IBAction) cancel:(id)sender
{
	[self close];
}

-(IBAction) savePreferences:(id)sender
{
	NSUserDefaults *defaults = 	[NSUserDefaults standardUserDefaults];
	[defaults setObject:[requestTokenUrl stringValue] forKey:OACRequestTokenUrl];
	[defaults setObject:[requestAuthUrl stringValue] forKey:OACRequestTokenAuthUrl];
	[defaults setObject:[accessTokenUrl stringValue] forKey:OACAccessTokenUrl];
	[defaults setObject:[consumerKey stringValue] forKey:OACConsumerKey];
	[defaults setObject:[consumerSecret stringValue] forKey:OACConsumerSecret];
	[self close];
}
@end
