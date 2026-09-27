"""Stage and ad-hoc sign the compiled device app, then package Payload only.
An ordinary iOS device still requires the sideloader's valid provisioning/signature.
"""
from pathlib import Path
import shutil,subprocess,zipfile,hashlib,json,plistlib
root=Path(__file__).resolve().parents[2]
app=root/'Recovered/iOS/bin/Release/net10.0-ios27.0/ios-arm64/IQuarters.iOS.app'
output=root/'dist';output.mkdir(exist_ok=True)
staging=output/'staging-realism-0.5.0/Payload/IQuarters.app'
if staging.exists(): shutil.rmtree(staging)
shutil.copytree(app,staging)
subprocess.run(['codesign','--force','--sign','-','--timestamp=none',str(staging)],check=True)
subprocess.run(['codesign','--verify','--deep','--strict',str(staging)],check=True)
ipa=output/'iQuarters-realism-0.5.0-ios15.ipa'
with zipfile.ZipFile(ipa,'w',zipfile.ZIP_DEFLATED,compresslevel=9) as archive:
 for file in sorted(staging.rglob('*')):
  if file.is_file():archive.write(file,file.relative_to(output/'staging-realism-0.5.0'))
with zipfile.ZipFile(ipa) as archive:
 assert archive.testzip() is None
 info=plistlib.loads(archive.read('Payload/IQuarters.app/Info.plist'))
 assert info['CFBundleShortVersionString']=='0.5.0' and info['CFBundleVersion']=='5'
 assert info['CFBundleDisplayName']=='iQuarters Realism'
 assert info['CFBundleIdentifier']=='com.itsgames.iquarters.realism'
 assert info['CADisableMinimumFrameDurationOnPhone'] is True
 assert info['MinimumOSVersion']=='15.0' and info['UIDeviceFamily']==[1,2]
 exe=archive.read('Payload/IQuarters.app/'+info['CFBundleExecutable']);assert exe[:4]==bytes.fromhex('cffaedfe')
 names=archive.namelist();assert not any('UnityEngine' in n or 'UnityFramework' in n for n in names)
metadata={'file':ipa.name,'bytes':ipa.stat().st_size,'sha256':hashlib.sha256(ipa.read_bytes()).hexdigest(),'minimumOS':info['MinimumOSVersion'],'architecture':'arm64','signature':'ad-hoc; conventional sideloaders must re-sign/provision','deviceLaunchTested':False,'status':'Game-style Graphics menu with recovered menu animations; device interaction unverified'}
(output/'build.json').write_text(json.dumps(metadata,indent=2))
print(json.dumps(metadata,indent=2))
