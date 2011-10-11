require 'erb'

class NUnitRunner
  include FileTest
  
  def initialize(paths)
    @sourceDir = paths.fetch(:source, 'source')
    @nunitDir = paths.fetch(:nunit, 'tools\\nunit')
    @resultsDir = paths.fetch(:results, 'results')
    @compileTarget = paths.fetch(:compilemode, 'debug')
  end
	
  def executeTests(assemblies)
    Dir.mkdir @resultsDir unless exists?(@resultsDir)
    
    assemblies.each do |assem|
      file = File.expand_path("#{@sourceDir}/#{assem}/bin/#{@compileTarget}/#{assem}.dll")
      sh "#{@nunitDir}\\nunit-console.exe #{file} /nothread /xml=#{@resultsDir}\\#{assem}.dll-results.xml"
    end
  end
end

class MSBuildRunner
  def initialize(clr_version)
    @clr_version = clr_version
  end
  
  def compile(attributes)
    compileTarget = attributes.fetch(:compilemode, 'debug')
    solutionFile = attributes[:solutionfile]
    
    frameworkDir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', @clr_version)
    msbuildFile = File.join(frameworkDir, 'msbuild.exe')
    sh "#{msbuildFile} #{solutionFile} /property:Configuration=#{compileTarget} /t:Rebuild"
  end

end

