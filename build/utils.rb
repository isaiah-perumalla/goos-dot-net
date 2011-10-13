require 'erb'

class NUnitRunner
  include FileTest
  
  def initialize(paths)
    @sourceDir = paths.fetch(:source, '../test')
    @nunitDir = paths.fetch(:nunit_dir, 'tools\\nunit')
    @resultsDir = paths.fetch(:results, 'output\\results')
    @compileTarget = paths.fetch(:compilemode, 'debug')
  end
	
  def executeTests(assemblies)
    Dir.mkdir @resultsDir unless exists?(@resultsDir)
    
    assemblies.each do |assem|
      file = File.expand_path("#{@sourceDir}/#{assem}/bin/#{@compileTarget}/#{assem}.dll")
      nunit_exe = File.join @nunitDir, 'nunit-console.exe'
      sh "#{nunit_exe} #{file} /nothread /xml=#{@resultsDir}\\#{assem}.dll-results.xml"
    end
  end
end

class MSBuildRunner
  def initialize(msbuild_dir)
    @msbuild_dir = msbuild_dir
  end
  
  def compile(attributes)
    compileTarget = attributes.fetch(:compilemode, 'debug')
    solutionFile = attributes[:solutionfile]
    
    msbuildFile = File.join(@msbuild_dir, 'msbuild.exe')
    sh "#{msbuildFile} #{solutionFile} /property:Configuration=#{compileTarget} /t:Rebuild"
  end

end

