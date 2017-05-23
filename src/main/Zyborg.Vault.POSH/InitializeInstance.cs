﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using VaultSharp.Backends.System.Models;

namespace Zyborg.Vault.POSH
{
	[Cmdlet(VerbsData.Initialize, "Instance")]
	public class InitializeInstance : VaultBaseCmdlet
	{
		[Parameter(Mandatory = false)]
		public int KeyShares
		{ get; set; } = 5;

		[Parameter(Mandatory = false)]
		public int KeyThreshold
		{ get; set; } = 3;

		protected override void BeginProcessing()
		{
			ResolveVaultClient();
		}

		protected override void ProcessRecord()
		{
			var opts = new InitializeOptions
			{
				SecretShares = KeyShares,
				SecretThreshold = KeyThreshold,
			};

			var r = AsyncWaitFor(_client.InitializeAsync(opts));

			int keyNum = 1;
			foreach (var k in r.Base64MasterKeys)
			{
				base.WriteObject(new KeyValuePair<string, string>($"UnsealKey_{keyNum++}", k));
			}

			base.WriteObject(new KeyValuePair<string, string>("InitialRootToken", r.RootToken));
		}
	}

	// This is a sample output from the CLI init command:
	//    C:\downloads\HashiCorp>vault init -key-shares=10 -key-threshold=2 -address=http://localhost:8200
	//
	//    Unseal Key 1: DCggsvq3l20uZmwUgqfFAYSyfpMOM6x6Xb8hCQ4JC+MB
	//    Unseal Key 2: xaGqF98te0VofjUtKdYGcKirQ1qeJGvaUbOkFZqltggC
	//    Unseal Key 3: gi8ldDVb1l2jdgI6uflHX0VVoR3uKd+6Vbcu6B/B3VED
	//    Unseal Key 4: TKilRpUCuBXkTodfZDSbkvCZOdOlCv6BSau1Lanm18UE
	//    Unseal Key 5: CyYqJX90FQ0vRrBI9BvavR1n25TVB0rhTa8/0CyCvJwF
	//    Unseal Key 6: wq+ggFru+SVpXulxX2oZzDF+5l1FEI1BQaO6zLguAXcG
	//    Unseal Key 7: hSEv47CYVD2iVt5mz0VY49yABBo1HTkhRacwMT1Kai4H
	//    Unseal Key 8: Rbq75AFcJbXnLvi7/uu6TUD9zdrTVs83eZuXXc9gFUQI
	//    Unseal Key 9: AjQ0h+sqiK0sJs+sbsT7Yq0DL52jW3tXfZ8doEoEfh0J
	//    Unseal Key 10: y72+Is6wZIVqPpaVxbU4E4EaElQzTLz3cZOYvN6ow/YK
	//    Initial Root Token: e763b6c0-49a7-7386-30c9-77a282f9d0f5
	//
	//    Vault initialized with 10 keys and a key threshold of 2. Please
	//    securely distribute the above keys.When the Vault is re-sealed,
	//    restarted, or stopped, you must provide at least 2 of these keys
	//    to unseal it again.
	//    
	//    Vault does not store the master key.Without at least 2 keys,
	//    your Vault will remain permanently sealed.
}
