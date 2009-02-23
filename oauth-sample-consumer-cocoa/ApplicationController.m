//
//  ApplicationController.m
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import "ApplicationController.h"

NSString * const OACConsumerKey = @"ConsumerKey";
NSString * const OACConsumerSecret = @"ConsumerSecret";
NSString * const OACRequestTokenKey = @"RequestTokenKey";
NSString * const OACRequestTokenSecret = @"RequestTokenSecret";
NSString * const OACRequestTokenUrl = @"RequestTokenURL";
NSString * const OACRequestTokenAuthUrl = @"RequestTokenAuthURL";
NSString * const OACAccessTokenUrl = @"AccessTokenURL";

@implementation ApplicationController

+(void) initialize
{
	NSMutableDictionary *defaultValues =[NSMutableDictionary dictionary];
	
	NSString *requestTokenUrl = @"http://term.ie/oauth/example/request_token.php";
	NSString *accessTokenUrl = @"http://term.ie/oauth/example/access_token.php";
	NSString *requestTokenAuthUrl = @"http://term.ie/oauth/example";
	NSString *consumerKey = @"key";
	NSString *consumerSecret = @"secret";

	[defaultValues setObject:requestTokenUrl forKey:OACRequestTokenUrl];
	[defaultValues setObject:accessTokenUrl forKey:OACAccessTokenUrl];
	[defaultValues setObject:requestTokenAuthUrl forKey:OACRequestTokenAuthUrl];
	[defaultValues setObject:consumerKey forKey:OACConsumerKey];
	[defaultValues setObject:consumerSecret forKey:OACConsumerSecret];
	
	[[NSUserDefaults standardUserDefaults] registerDefaults:defaultValues];
	
}


-(IBAction) showPreferenceController:(id)sender
{
	if(!preferenceController){
		preferenceController = [[PreferenceController alloc]init];		
	}
	[preferenceController showWindow:self];
}
@end
